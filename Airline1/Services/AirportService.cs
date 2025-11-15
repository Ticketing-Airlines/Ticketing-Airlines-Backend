using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class AirportService(IAirportRepository repo, IMapper mapper) : IAirportService
    {
        private readonly IAirportRepository _repo = repo;
        private readonly IMapper _mapper = mapper;

        public async Task<AirportResponse> CreateAsync(CreateAirportRequest request)
        {
            var model = _mapper.Map<Airport>(request);
            model.CreatedAt = DateTime.UtcNow;
            var added = await _repo.AddAsync(model);
            return _mapper.Map<AirportResponse>(added);
        }

        public async Task<List<AirportResponse>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return [.. items.Select(i => _mapper.Map<AirportResponse>(i))];
        }

        public async Task<AirportResponse?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<AirportResponse>(item);
        }

        public async Task<AirportResponse?> UpdateAsync(int id, UpdateAirportRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            // Map non-null fields from request onto existing
            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
            return _mapper.Map<AirportResponse>(existing);
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
