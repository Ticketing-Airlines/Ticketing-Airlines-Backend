using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightBundleService(IFlightBundleRepository repo, IMapper mapper) : IFlightBundleService
    {
        public async Task<IEnumerable<FlightBundleResponse>> GetAllAsync()
        {
            var bundles = await repo.GetAllAsync();
            return mapper.Map<IEnumerable<FlightBundleResponse>>(bundles);
        }

        public async Task<FlightBundleResponse?> GetByIdAsync(int id)
        {
            var bundle = await repo.GetByIdAsync(id);
            return bundle == null ? null : mapper.Map<FlightBundleResponse>(bundle);
        }

        public async Task<FlightBundleResponse> CreateAsync(CreateFlightBundleRequest request)
        {
            var bundle = mapper.Map<FlightBundle>(request);
            await repo.AddAsync(bundle);
            await repo.SaveChangesAsync(); // Commit the changes to the database
            return mapper.Map<FlightBundleResponse>(bundle);
        }

        public async Task<FlightBundleResponse?> UpdateAsync(int id, UpdateFlightBundleRequest request)
        {
            var bundle = await repo.GetByIdAsync(id);
            if (bundle == null) return null;

            mapper.Map(request, bundle);
            await repo.UpdateAsync(bundle);
            await repo.SaveChangesAsync(); // Commit the changes to the database

            return mapper.Map<FlightBundleResponse>(bundle);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bundle = await repo.GetByIdAsync(id);
            if (bundle == null) return false;

            // FIX: Pass the 'id' (int) argument, as defined in the IFlightBundleRepository
            await repo.DeleteAsync(id);

            await repo.SaveChangesAsync(); // Commit the deletion to the database

            return true;
        }
    }
}