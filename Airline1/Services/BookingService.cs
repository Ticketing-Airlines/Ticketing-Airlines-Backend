using Airline1.Data;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline1.Services
{
    public class BookingService(
        IBookingRepository bookingRepo,
        IFlightPriceService flightPriceService,
        IAddOnPriceService addOnPriceService,
        IFlightSeatService flightSeatService,
        IFlightBundleRepository flightBundleRepo,
        IAddOnPriceRepository addOnPriceRepo,
        AppDbContext db,
        IMapper mapper) : IBookingService
    {
        // Private method to compute the total price before creating the booking
        public async Task<decimal> CalculateTotalCostAsync(CreateBookingRequest request)
        {
            decimal totalCost = 0m;
            var now = DateTime.UtcNow;

            // 1. Get Flight Bundle Price Increment
            var bundle = await flightBundleRepo.GetByIdAsync(request.FlightBundleId)
                ?? throw new KeyNotFoundException($"FlightBundle with ID {request.FlightBundleId} not found.");

            foreach (var flightId in request.FlightIds)
            {

                // 2. Calculate Base Fare per passenger type
                foreach (var passengerRequest in request.Passengers)
                {
                    var priceResponse = await flightPriceService.GetCurrentPriceAsync(
                       flightId,
                        "Economy", // Assuming default CabinClass for now
                        request.FlightBundleId,
                        passengerRequest.PassengerType,
                        now) ?? throw new InvalidOperationException($"Base price not found for Flight {request.FlightIds.FirstOrDefault()}, Bundle {request.FlightBundleId}, Type {passengerRequest.PassengerType}.");

                    // Base price + Bundle Increment
                    decimal passengerBaseFare = priceResponse.BasePrice + bundle.PriceIncrement;
                    totalCost += passengerBaseFare;

                    // 3. Calculate Add-Ons (Baggage, Meals, etc.)
                    if (passengerRequest.AddOnPriceIds.Count != 0)
                    {
                        // Call the service to calculate the active cost of the requested add-ons
                        totalCost += await addOnPriceService.GetTotalCostByIdsAsync(passengerRequest.AddOnPriceIds);
                    }

                    // 4. Calculate Seat Cost (The FlightSeat.PriceAmount is what links to the cost)
                    if (passengerRequest.FlightSeatId.HasValue)
                    {
                        // NOTE: This call only retrieves the seat. The actual reservation logic and checks happen in CreateAsync.
                        var fs = await flightSeatService.GetByIdAsync(passengerRequest.FlightSeatId.Value)
                            ?? throw new KeyNotFoundException($"FlightSeat with ID {passengerRequest.FlightSeatId.Value} not found.");

                        if (fs.PriceAmount.HasValue)
                        {
                            totalCost += fs.PriceAmount.Value;
                        }
                    }
                }

               
            }
             return totalCost;
        }
 
        public async Task<BookingResponse> CreateAsync(CreateBookingRequest request)
        {
            // Use an explicit transaction to ensure all database writes succeed or fail together
            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                // --- 1. Calculate Total Price and Basic Validation ---
                decimal calculatedPrice = await CalculateTotalCostAsync(request);
                string pnr = bookingRepo.GenerateUniquePnr();

                // --- 2. Create the Booking Entity ---
                var booking = new Booking
                {
                    Pnr = pnr,
                    FlightBundleId = request.FlightBundleId,
                    UserId = request.UserId,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    TotalPrice = calculatedPrice,
                    Currency = "PHP", // Hardcoded currency for now
                    Status = "PendingPayment",
                    BookingDate = DateTime.UtcNow
                };

                foreach(var fId in request.FlightIds)
                {
                    booking.BookingFlights.Add(new BookingFlight
                    {
                        FlightId = fId
                    });
                }

                await bookingRepo.AddAsync(booking);
                await bookingRepo.SaveChangesAsync(); // Get the generated BookingId

                // --- 3. Process Passengers, Add-Ons, and Reserve Seats ---
                foreach (var pReq in request.Passengers)
                {
                    var passenger = mapper.Map<BookingPassenger>(pReq);
                    passenger.BookingId = booking.BookingId;

                    // Add and Save the Passenger FIRST to get the BookingPassengerId
                    // This is CRITICAL for the foreign keys on BookingAddOn and FlightSeat.
                    db.Set<BookingPassenger>().Add(passenger);
                    await bookingRepo.SaveChangesAsync();

                    // --- 4. Process Add-Ons ---
                    if (pReq.AddOnPriceIds.Count != 0)
                    {
                        // Get the active prices at the time of booking
                        var prices = await addOnPriceRepo.GetPricesByIdsAsync(pReq.AddOnPriceIds);

                        foreach (var price in prices)
                        {
                            // FIX: Set the required 'PassengerId' property using the newly generated ID.
                            db.Set<BookingAddOn>().Add(new BookingAddOn
                            {
                                PassengerId = passenger.BookingPassengerId, // <-- FIX APPLIED HERE
                                AddOnPriceId = price.AddOnPriceId,
                                PriceAtBooking = price.PriceAmount // Denormalize the price
                            });
                        }
                    }

                    // --- 5. Finalize Seat Reservation (Setting the correct PassengerId on FlightSeat) ---
                    if (pReq.FlightSeatId.HasValue)
                    {
                        // The ReserveSeatAsync service method is called to update the FlightSeat Status and link it to the newly created BookingPassengerId
                        await flightSeatService.ReserveSeatAsync(pReq.FlightSeatId.Value,
                            new ReserveFlightSeatRequest
                            {
                                BookingId = booking.BookingId,
                                PassengerId = passenger.BookingPassengerId, // <-- FIX APPLIED HERE
                                SeatAddOnId = null
                            });
                    }
                }

                // Save all pending changes (Add-ons and Final Seat Updates)
                await bookingRepo.SaveChangesAsync();

                // Commit the entire transaction
                await transaction.CommitAsync();

                // --- 6. Final Response Mapping ---
                // Fetch the fully populated booking entity again for a clean response map
                var createdBooking = await bookingRepo.GetByIdAsync(booking.BookingId)
                    ?? throw new InvalidOperationException("Failed to retrieve created booking after commit.");

                return mapper.Map<BookingResponse>(createdBooking);
            }
            catch (Exception)
            {
                // Rollback transaction on any failure
                await transaction.RollbackAsync();
                throw; // Re-throw the exception for the controller to handle
            }
        }

        // --- Retrieval Methods ---

        public async Task<BookingResponse?> GetByIdAsync(int id)
        {
            var booking = await bookingRepo.GetByIdAsync(id);
            return booking == null ? null : mapper.Map<BookingResponse>(booking);
        }

        public async Task<BookingResponse?> GetByPnrAsync(string pnr)
        {
            var booking = await bookingRepo.GetByPnrAsync(pnr);
            return booking == null ? null : mapper.Map<BookingResponse>(booking);
        }

        public async Task<IEnumerable<BookingResponse>> GetByUserIdAsync(int userId)
        {
            var bookings = await bookingRepo.GetByUserIdAsync(userId);
            return mapper.Map<IEnumerable<BookingResponse>>(bookings);
        }

        // --- Update Methods ---

        public async Task<BookingResponse?> UpdateStatusAsync(string pnr, string newStatus)
        {
            var booking = await bookingRepo.GetByPnrAsync(pnr);
            if (booking == null) return null;

            // Simple state machine validation (e.g., cannot go from Cancelled to Confirmed)
            // NOTE: Full state machine logic is omitted for brevity.
            if (booking.Status == "Cancelled" && newStatus != "Cancelled")
                throw new InvalidOperationException("Cannot change status of a Cancelled booking.");

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            // Special case: If confirmed, set PaymentDate
            if (newStatus == "Confirmed" && !booking.PaymentDate.HasValue)
            {
                booking.PaymentDate = DateTime.UtcNow;
            }

            await bookingRepo.UpdateAsync(booking);
            await bookingRepo.SaveChangesAsync();
            return mapper.Map<BookingResponse>(booking);
        }

        public async Task<BookingResponse?> UpdateContactInfoAsync(string pnr, UpdateBookingRequest request)
        {
            var booking = await bookingRepo.GetByPnrAsync(pnr);
            if (booking == null) return null;

            if (request.ContactEmail != null) booking.ContactEmail = request.ContactEmail;
            if (request.ContactPhone != null) booking.ContactPhone = request.ContactPhone;

            booking.UpdatedAt = DateTime.UtcNow;

            await bookingRepo.UpdateAsync(booking);
            await bookingRepo.SaveChangesAsync();
            return mapper.Map<BookingResponse>(booking);
        }
    }
}