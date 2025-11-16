using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightRouteService(IFlightRouteRepository repo, IAirportRepository airportRepo, IMapper mapper) : IFlightRouteService
    {
        public async Task<FlightRouteResponse> CreateAsync(CreateFlightRouteRequest request)
        {
            if (!await airportRepo.ExistsAsync(request.OriginAirportId))
                throw new KeyNotFoundException($"Origin airport id {request.OriginAirportId} not found.");

            if (!await airportRepo.ExistsAsync(request.DestinationAirportId))
                throw new KeyNotFoundException($"Destination airport id {request.DestinationAirportId} not found.");

            var existing = await repo.GetByOriginDestinationAsync(request.OriginAirportId, request.DestinationAirportId);
            if (existing != null)
                throw new InvalidOperationException("Route between these airports already exists.");

            var model = mapper.Map<FlightRoute>(request);
            model.CreatedAt = DateTime.UtcNow;

            var added = await repo.AddAsync(model);
            return mapper.Map<FlightRouteResponse>(added);
        }

        public async Task<List<FlightRouteResponse>> GetAllAsync()
        {
            var items = await repo.GetAllAsync();
            return [.. items.Select(i => mapper.Map<FlightRouteResponse>(i))];
        }

        public async Task<FlightRouteResponse> GetByIdAsync(int id)
        {
            var item = await repo.GetByIdAsync(id);
            return item == null
                ? throw new KeyNotFoundException($"Flight route with id {id} not found.")
                : mapper.Map<FlightRouteResponse>(item);
        }

        public async Task<FlightRouteResponse> UpdateAsync(int id, UpdateFlightRouteRequest request)
        {
            var existing = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Flight route with id {id} not found.");
            if (request.OriginAirportId.HasValue && !await airportRepo.ExistsAsync(request.OriginAirportId.Value))
                throw new KeyNotFoundException($"Origin airport id {request.OriginAirportId.Value} not found.");

            if (request.DestinationAirportId.HasValue && !await airportRepo.ExistsAsync(request.DestinationAirportId.Value))
                throw new KeyNotFoundException($"Destination airport id {request.DestinationAirportId.Value} not found.");

            var newOrigin = request.OriginAirportId ?? existing.OriginAirportId;
            var newDest = request.DestinationAirportId ?? existing.DestinationAirportId;

            var duplicate = await repo.GetByOriginDestinationAsync(newOrigin, newDest);
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("Another route with the same origin and destination already exists.");

            mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            await repo.UpdateAsync(existing);

            return mapper.Map<FlightRouteResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Flight route with id {id} not found.");
            await repo.DeleteAsync(existing);
            return true;
        }
    }
}
