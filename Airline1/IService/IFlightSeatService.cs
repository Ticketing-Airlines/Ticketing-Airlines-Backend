using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightSeatService
    {
        Task InitializeSeatsForFlightAsync(int flightId);
        Task<FlightSeatResponse> ReserveSeatAsync(int flightSeatId, ReserveFlightSeatRequest request);
        Task<FlightSeatResponse> AssignSeatAsync(int flightSeatId, AssignFlightSeatRequest request);
        Task<FlightSeatResponse> BlockSeatAsync(int flightSeatId, BlockFlightSeatRequest request);
        Task<IEnumerable<FlightSeatResponse>> GetByFlightAsync(int flightId);
        Task<FlightSeatResponse> GetByIdAsync(int id);
    }
}
