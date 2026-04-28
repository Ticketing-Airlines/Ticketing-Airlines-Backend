using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface ICheckInRepository
    {
        Task<Booking?> GetBookingForCheckInAsync(string pnr);
        Task<List<CheckIn>> GetCheckInsByBookingIdAsync(Guid bookingId);
        Task<CheckIn?> GetCheckInByPassengerIdAsync(Guid passengerId);
        Task<FlightSeat?> GetFlightSeatByFlightAndSeatNumberAsync(int flightId, string seatNumber);
        Task<List<FlightSeat>> GetAvailableSeatsForFlightAsync(int flightId, int limit);
        Task AddCheckInAsync(CheckIn checkIn);
        Task AddBoardingPassAsync(BoardingPass boardingPass);
        Task UpdateFlightSeatAsync(FlightSeat flightSeat);
        Task UpdateBookingAsync(Booking booking);
        Task SaveChangesAsync();
    }
}