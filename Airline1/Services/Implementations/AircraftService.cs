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
    public class AircraftService : IAircraftService
    {
        private readonly IAircraftRepository _repo;
        private readonly IMapper _mapper;

        public AircraftService(IAircraftRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<AircraftResponse> CreateAsync(CreateAircraftRequest request)
        {
            // optional: check duplicate tail number
            var exists = await _repo.GetByTailNumberAsync(request.TailNumber);
            if (exists != null)
                throw new InvalidOperationException($"Tail number '{request.TailNumber}' already exists.");

            var model = _mapper.Map<Aircraft>(request);
            model.CreatedAt = DateTime.UtcNow;

            var added = await _repo.AddAsync(model);
            return _mapper.Map<AircraftResponse>(added);
        }

        public async Task<List<AircraftResponse>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return items.Select(i => _mapper.Map<AircraftResponse>(i)).ToList();
        }

        public async Task<AircraftResponse?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return null;
            return _mapper.Map<AircraftResponse>(item);
        }

        public async Task<AircraftResponse?> UpdateAsync(int id, UpdateAircraftRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            // Map non-null members from request => existing
            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
            return _mapper.Map<AircraftResponse>(existing);
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
