using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightSeatService
    {
        Task InitializeSeatsForFlightAsync(int flightId);
        Task<FlightSeatResponse> ReserveSeatAsync(Guid flightSeatId, ReserveFlightSeatRequest request);
        Task<FlightSeatResponse> AssignSeatAsync(Guid flightSeatId, AssignFlightSeatRequest request);
        Task<FlightSeatResponse> BlockSeatAsync(Guid flightSeatId, BlockFlightSeatRequest request);
        Task<IEnumerable<FlightSeatResponse>> GetByFlightAsync(int flightId);
        Task<FlightSeatResponse> GetByIdAsync(Guid id);
    }
}
