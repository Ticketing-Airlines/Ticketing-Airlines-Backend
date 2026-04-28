using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightService
    {
        Task<IEnumerable<FlightResponse>> GetAllAsync();
        Task<FlightResponse?> GetByIdAsync(int id);
        Task<FlightResponse> CreateAsync(CreateFlightRequest request);
        Task<FlightResponse?> UpdateAsync(int id, UpdateFlightRequest request);

        Task<FlightResponse?> GetFlightStatusAsync(string flightNumber, DateTime date);

        Task<FlightStatusApiResponse> GetFlightStatusLookupAsync(string flightNumber, string date);

        Task<IEnumerable<FlightResponse>> SearchFlightsAsync(string origin, string destination, DateTime departureDate, int passengerCount);

        // New enhanced flight search for frontend alignment
        Task<FlightSearchResponse> SearchFlightsAsync(SearchFlightRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
