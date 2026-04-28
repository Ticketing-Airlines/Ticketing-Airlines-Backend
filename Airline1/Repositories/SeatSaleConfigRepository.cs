using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Airline1.Repositories
{
    public class SeatSaleConfigRepository(AppDbContext db) : ISeatSaleConfigRepository
    {
        private readonly AppDbContext _db = db;

        public async Task<SeatSaleConfig?> GetConfigAsync()
        {
            return await _db.Set<SeatSaleConfig>()
                .Include(c => c.TermsAndConditions)
                .FirstOrDefaultAsync();
        }

        public async Task<SeatSaleConfig> AddAsync(SeatSaleConfig config)
        {
            await _db.Set<SeatSaleConfig>().AddAsync(config);
            return config;
        }

        public Task UpdateAsync(SeatSaleConfig config)
        {
            _db.Set<SeatSaleConfig>().Update(config);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
