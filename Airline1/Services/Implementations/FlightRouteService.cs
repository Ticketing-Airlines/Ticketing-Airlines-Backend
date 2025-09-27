using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Models;
using Airline1.Repositories.Interfaces;
using Airline1.Services.Interfaces;
using AutoMapper;

namespace Airline1.Services.Implementations
{
    public class FlightRouteService : IFlightRouteService
    {
        private readonly IFlightRouteRepository _repo;
        private readonly IAirportRepository _airportRepo;
        private readonly IMapper _mapper;

        public FlightRouteService(IFlightRouteRepository repo, IAirportRepository airportRepo, IMapper mapper)
        {
            _repo = repo;
            _airportRepo = airportRepo;
            _mapper = mapper;
        }

        public async Task<FlightRouteResponse> CreateAsync(CreateFlightRouteRequest request)
        {
            if (!await _airportRepo.ExistsAsync(request.OriginAirportId))
                throw new InvalidOperationException($"Origin airport id {request.OriginAirportId} not found.");

            if (!await _airportRepo.ExistsAsync(request.DestinationAirportId))
                throw new InvalidOperationException($"Destination airport id {request.DestinationAirportId} not found.");

            var existing = await _repo.GetByOriginDestinationAsync(request.OriginAirportId, request.DestinationAirportId);
            if (existing != null)
                throw new InvalidOperationException("Route between these airports already exists.");

            var model = _mapper.Map<FlightRoute>(request);
            model.CreatedAt = DateTime.UtcNow;

            var added = await _repo.AddAsync(model);
            return _mapper.Map<FlightRouteResponse>(added);
        }

        public async Task<List<FlightRouteResponse>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return items.Select(i => _mapper.Map<FlightRouteResponse>(i)).ToList();
        }

        public async Task<FlightRouteResponse> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<FlightRouteResponse>(item);
        }

        public async Task<FlightRouteResponse> UpdateAsync(int id, UpdateFlightRouteRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (request.OriginAirportId.HasValue && !await _airportRepo.ExistsAsync(request.OriginAirportId.Value))
                throw new InvalidOperationException($"Origin airport id {request.OriginAirportId.Value} not found.");

            if (request.DestinationAirportId.HasValue && !await _airportRepo.ExistsAsync(request.DestinationAirportId.Value))
                throw new InvalidOperationException($"Destination airport id {request.DestinationAirportId.Value} not found.");

            var newOrigin = request.OriginAirportId ?? existing.OriginAirportId;
            var newDest = request.DestinationAirportId ?? existing.DestinationAirportId;
            var duplicate = await _repo.GetByOriginDestinationAsync(newOrigin, newDest);
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("Another route with the same origin and destination already exists.");

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
            return _mapper.Map<FlightRouteResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            await _repo.DeleteAsync(existing);
            return true;
        }
    }
}
