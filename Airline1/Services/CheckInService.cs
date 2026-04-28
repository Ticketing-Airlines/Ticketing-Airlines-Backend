using Airline1.Common;
using Airline1.Data;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Airline1.Services
{
    public class CheckInService(
        ICheckInRepository checkInRepo,
        IBookingRepository bookingRepo,
        AppDbContext db,
        ILogger<CheckInService> logger) : ICheckInService
    {
        public async Task<CheckInEligibilityResponse> GetEligibilityAsync(string bookingReference)
        {
            var booking = await checkInRepo.GetBookingForCheckInAsync(bookingReference);

            if (booking == null)
            {
                return new CheckInEligibilityResponse
                {
                    BookingReference = bookingReference,
                    IsEligible = false,
                    Message = "Booking not found"
                };
            }

            // Get the first flight from the booking
            var firstBookingFlight = booking.BookingFlights.FirstOrDefault();
            if (firstBookingFlight?.Flight == null)
            {
                return new CheckInEligibilityResponse
                {
                    BookingReference = bookingReference,
                    IsEligible = false,
                    Message = "No flight information found"
                };
            }

            var flight = firstBookingFlight.Flight;
            var route = flight.Route;
            if (route == null)
            {
                return new CheckInEligibilityResponse
                {
                    BookingReference = bookingReference,
                    IsEligible = false,
                    Message = "Flight route information not found"
                };
            }

            // Determine domestic vs international
            bool isDomestic = string.Equals(
                route.OriginAirport?.Country,
                route.DestinationAirport?.Country,
                StringComparison.OrdinalIgnoreCase);
            string flightType = isDomestic ? "Domestic" : "International";

            // Calculate check-in window
            var departureTime = flight.DepartureTime;
            var opensAt = isDomestic
                ? departureTime.AddHours(-24)
                : departureTime.AddHours(-48);
            var closesAt = isDomestic
                ? departureTime.AddHours(-1)
                : departureTime.AddHours(-2);

            var now = DateTime.UtcNow;

            // Check if flight is cancelled
            bool isFlightCancelled = flight.Statuses != null
                && flight.Statuses.Any(s => s.IsCurrent && s.Status == FlightStatusType.Cancelled);

            // Determine eligibility
            bool isEligible = now >= opensAt
                && now <= closesAt
                && (booking.Status == "Confirmed" || booking.Status == "Paid")
                && !isFlightCancelled;

            string? message = null;
            if (!isEligible)
            {
                if (isFlightCancelled)
                    message = "Flight has been cancelled";
                else if (booking.Status != "Confirmed" && booking.Status != "Paid")
                    message = $"Booking status is '{booking.Status}', check-in requires 'Confirmed' or 'Paid'";
                else if (now < opensAt)
                    message = $"Check-in opens at {opensAt:yyyy-MM-dd HH:mm} UTC";
                else if (now > closesAt)
                    message = "Check-in window has closed";
            }

            return new CheckInEligibilityResponse
            {
                BookingReference = bookingReference,
                IsEligible = isEligible,
                Message = message,
                Flight = new FlightInfoForCheckIn
                {
                    FlightNumber = flight.FlightNumber,
                    Origin = route.OriginAirport?.IataCode ?? route.OriginAirportId.ToString(),
                    Destination = route.DestinationAirport?.IataCode ?? route.DestinationAirportId.ToString(),
                    DepartureTime = flight.DepartureTime,
                    FlightType = flightType
                },
                CheckInWindow = new CheckInWindow
                {
                    OpensAt = opensAt,
                    ClosesAt = closesAt
                }
            };
        }

        public async Task<CheckInVerifyResponse> VerifyAsync(CheckInVerifyRequest request)
        {
            // Validate booking reference format: exactly 6 alphanumeric characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.BookingReference, @"^[A-Z0-9]{6}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                throw new InvalidOperationException("Invalid booking reference format");
            }

            var booking = await checkInRepo.GetBookingForCheckInAsync(request.BookingReference);

            if (booking == null)
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "BOOKING_NOT_FOUND"
                };
            }

            // Primary passenger is the first passenger ordered by BookingPassengerId
            var primaryPassenger = booking.Passengers
                .OrderBy(p => p.BookingPassengerId)
                .First();

            // Case-insensitive last name comparison
            if (!string.Equals(primaryPassenger.LastName, request.LastName, StringComparison.OrdinalIgnoreCase))
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "CHECK_IN_NOT_AVAILABLE"
                };
            }

            // Booking status must be "Confirmed" or "Paid" (case-sensitive as stored)
            if (booking.Status != "Confirmed" && booking.Status != "Paid")
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "CHECK_IN_NOT_AVAILABLE"
                };
            }

            // Check for existing check-ins
            var existingCheckIns = await checkInRepo.GetCheckInsByBookingIdAsync(booking.BookingId);
            if (existingCheckIns.Any(c => c.Status == "CheckedIn"))
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "ALREADY_CHECKED_IN"
                };
            }

            // Determine check-in window
            var firstBookingFlight = booking.BookingFlights.FirstOrDefault();
            if (firstBookingFlight?.Flight == null)
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "CHECK_IN_NOT_AVAILABLE"
                };
            }

            var flight = firstBookingFlight.Flight;
            var route = flight.Route;
            if (route == null)
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "CHECK_IN_NOT_AVAILABLE"
                };
            }

            bool isDomestic = string.Equals(
                route.OriginAirport?.Country,
                route.DestinationAirport?.Country,
                StringComparison.OrdinalIgnoreCase);

            var departureTime = flight.DepartureTime;
            var closesAt = isDomestic
                ? departureTime.AddHours(-1)
                : departureTime.AddHours(-2);

            // Check if check-in window has closed
            if (DateTime.UtcNow > closesAt)
            {
                return new CheckInVerifyResponse
                {
                    BookingReference = request.BookingReference,
                    IsVerified = false,
                    ErrorCode = "CHECK_IN_NOT_AVAILABLE"
                };
            }

            // Build comprehensive response
            var passengerCheckIns = existingCheckIns
                .Where(c => c.PassengerId.HasValue)
                .ToDictionary(c => c.PassengerId!.Value, c => c.Status == "CheckedIn");

            var passengers = booking.Passengers
                .OrderBy(p => p.BookingPassengerId)
                .Select(p => new PassengerForCheckIn
                {
                    PassengerId = p.BookingPassengerId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    MiddleName = p.MiddleName,
                    PassengerType = p.PassengerType,
                    SeatNumber = p.FlightSeat?.Seat?.SeatNumber,
                    IsCheckedIn = passengerCheckIns.TryGetValue(p.BookingPassengerId, out var isCheckedIn) && isCheckedIn
                })
                .ToList();

            var opensAt = isDomestic
                ? departureTime.AddHours(-24)
                : departureTime.AddHours(-48);

            var flights = booking.BookingFlights
                .Where(bf => bf.Flight != null)
                .Select(bf => new FlightInBooking
                {
                    FlightNumber = bf.Flight!.FlightNumber,
                    Origin = bf.Flight.Route?.OriginAirport?.IataCode ?? bf.Flight.RouteId.ToString(),
                    Destination = bf.Flight.Route?.DestinationAirport?.IataCode ?? bf.Flight.RouteId.ToString(),
                    DepartureTime = bf.Flight.DepartureTime,
                    ArrivalTime = bf.Flight.ArrivalTime
                })
                .ToList();

            return new CheckInVerifyResponse
            {
                BookingReference = request.BookingReference,
                IsVerified = true,
                ErrorCode = null,
                Booking = new BookingDetailsForCheckIn
                {
                    Pnr = booking.Pnr,
                    Status = booking.Status,
                    TotalPrice = booking.TotalPrice,
                    Currency = booking.Currency,
                    Flights = flights
                },
                Passengers = passengers,
                Eligibility = new CheckInEligibilityInfo
                {
                    IsEligible = true,
                    CheckInOpensAt = opensAt,
                    CheckInClosesAt = closesAt,
                    Reason = null
                }
            };
        }

        public async Task<CheckInCompleteResponse> CompleteAsync(CheckInCompleteRequest request)
        {
            var booking = await checkInRepo.GetBookingForCheckInAsync(request.BookingReference)
                ?? throw new KeyNotFoundException($"Booking with reference '{request.BookingReference}' not found.");

            // Validate all passengers in the payload exist in the booking
            var bookingPassengerIds = booking.Passengers.Select(p => p.BookingPassengerId).ToHashSet();
            foreach (var pax in request.Passengers)
            {
                if (!bookingPassengerIds.Contains(pax.PassengerId))
                {
                    throw new InvalidOperationException($"Passenger with ID '{pax.PassengerId}' is not part of this booking.");
                }
            }

            // Ensure every passenger has a seat number assigned
            foreach (var pax in request.Passengers)
            {
                if (string.IsNullOrWhiteSpace(pax.SeatNumber))
                {
                    throw new InvalidOperationException($"Passenger with ID '{pax.PassengerId}' must have a seat number assigned.");
                }
            }

            // Get the first flight from the booking
            var firstBookingFlight = booking.BookingFlights.FirstOrDefault()
                ?? throw new InvalidOperationException("Booking has no flights assigned.");

            var flight = firstBookingFlight.Flight
                ?? throw new InvalidOperationException("Flight information not found for booking.");

            int flightId = firstBookingFlight.FlightId;

            // Pre-validate seat availability
            foreach (var pax in request.Passengers)
            {
                var flightSeat = await checkInRepo.GetFlightSeatByFlightAndSeatNumberAsync(flightId, pax.SeatNumber);
                if (flightSeat == null || flightSeat.Status != "Available")
                {
                    var alternatives = await checkInRepo.GetAvailableSeatsForFlightAsync(flightId, 5);
                    var alternativeSeatNumbers = alternatives
                        .Where(s => s.Seat != null)
                        .Select(s => s.Seat!.SeatNumber)
                        .ToList();

                    throw new InvalidOperationException($"SEAT_UNAVAILABLE:{string.Join(",", alternativeSeatNumbers)}");
                }
            }

            // Use a database transaction for atomicity
            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var boardingPasses = new List<BoardingPassResponse>();

                foreach (var pax in request.Passengers)
                {
                    var flightSeat = await checkInRepo.GetFlightSeatByFlightAndSeatNumberAsync(flightId, pax.SeatNumber)
                        ?? throw new InvalidOperationException($"Flight seat '{pax.SeatNumber}' not found for flight {flightId}.");

                    // Update FlightSeat status
                    flightSeat.Status = "CheckedIn";
                    flightSeat.BookingId = booking.BookingId;
                    flightSeat.PassengerId = pax.PassengerId;
                    flightSeat.UpdatedAt = DateTime.UtcNow;
                    await checkInRepo.UpdateFlightSeatAsync(flightSeat);

                    // Create CheckIn record
                    var checkIn = new CheckIn
                    {
                        BookingId = booking.BookingId,
                        PassengerId = pax.PassengerId,
                        SeatNumber = pax.SeatNumber,
                        Status = "CheckedIn",
                        CreatedAt = DateTime.UtcNow
                    };
                    await checkInRepo.AddCheckInAsync(checkIn);

                    // Generate Barcode: {BookingRef}{FlightNumber}{FromAirport}{DepartureDate:yyyyMMdd}
                    var fromAirport = flight.Route?.OriginAirport?.IataCode ?? flight.RouteId.ToString();
                    var barcode = $"{booking.Pnr}{flight.FlightNumber}{fromAirport}{flight.DepartureTime:yyyyMMdd}";

                    // Generate QR code
                    var boardingPassUrl = $"/boarding-pass/{checkIn.CheckInId}";
                    var qrGenerator = new QRCodeGenerator();
                    var qrCodeData = qrGenerator.CreateQrCode(boardingPassUrl, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new PngByteQRCode(qrCodeData);
                    var qrCodeBytes = qrCode.GetGraphic(20);
                    var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

                    // Get passenger name
                    var passenger = booking.Passengers.First(p => p.BookingPassengerId == pax.PassengerId);
                    var passengerName = $"{passenger.FirstName} {passenger.LastName}";
                    var toAirport = flight.Route?.DestinationAirport?.IataCode ?? flight.RouteId.ToString();

                    // Create BoardingPass record
                    var boardingPass = new BoardingPass
                    {
                        CheckInId = checkIn.CheckInId,
                        BookingReference = booking.Pnr,
                        FlightNumber = flight.FlightNumber,
                        PassengerName = passengerName,
                        FromAirport = fromAirport,
                        ToAirport = toAirport,
                        DepartureDate = flight.DepartureTime,
                        SeatNumber = pax.SeatNumber,
                        Barcode = barcode,
                        QRCodeImageBase64 = qrCodeBase64,
                        BoardingPassUrl = boardingPassUrl,
                        CreatedAt = DateTime.UtcNow
                    };
                    await checkInRepo.AddBoardingPassAsync(boardingPass);

                    boardingPasses.Add(new BoardingPassResponse
                    {
                        BoardingPassId = boardingPass.BoardingPassId,
                        PassengerName = passengerName,
                        FlightNumber = flight.FlightNumber,
                        FromAirport = fromAirport,
                        ToAirport = toAirport,
                        DepartureDate = flight.DepartureTime,
                        SeatNumber = pax.SeatNumber,
                        Barcode = barcode,
                        QRCodeImageBase64 = qrCodeBase64,
                        BoardingPassUrl = boardingPassUrl
                    });
                }

                // Update booking status
                booking.Status = "Checked In";
                booking.UpdatedAt = DateTime.UtcNow;
                await checkInRepo.UpdateBookingAsync(booking);

                // Save all changes and commit transaction
                await checkInRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                // Send check-in confirmation notification (placeholder)
                await SendCheckInConfirmationAsync(booking.BookingId, boardingPasses);

                return new CheckInCompleteResponse
                {
                    BookingReference = booking.Pnr,
                    Status = booking.Status,
                    CheckedInAt = DateTime.UtcNow,
                    BoardingPasses = boardingPasses
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private Task SendCheckInConfirmationAsync(Guid bookingId, List<BoardingPassResponse> boardingPasses)
        {
            logger.LogInformation("Check-in confirmation for booking {BookingId} with {PassengerCount} passenger(s). Boarding passes generated.",
                bookingId, boardingPasses.Count);
            return Task.CompletedTask;
        }
    }
}