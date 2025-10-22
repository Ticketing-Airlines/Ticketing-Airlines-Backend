using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightService(IFlightRepository repository, IMapper mapper) : IFlightService
    {
        public async Task<IEnumerable<FlightResponse>> GetAllAsync()
        {
            var flights = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<FlightResponse>>(flights);
        }

        public async Task<FlightResponse?> GetByIdAsync(int id)
        {
            var flight = await repository.GetByIdAsync(id);
            return flight == null ? null : mapper.Map<FlightResponse>(flight);
        }

        public async Task<FlightResponse> CreateAsync(CreateFlightRequest request)
        {
            var flight = mapper.Map<Flight>(request);
            await repository.AddAsync(flight);
            await repository.SaveChangesAsync();
            return mapper.Map<FlightResponse>(flight);
        }

        public async Task<FlightResponse?> UpdateAsync(int id, UpdateFlightRequest request)
        {
            var flight = await repository.GetByIdAsync(id);
            if (flight == null) return null;

            mapper.Map(request, flight);
            repository.UpdateAsync(flight);
            await repository.SaveChangesAsync();
            return mapper.Map<FlightResponse>(flight);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var flight = await repository.GetByIdAsync(id);
            if (flight == null) return false;

            repository.DeleteAsync(flight);
            await repository.SaveChangesAsync();
            return true;
        }
    }
}
