using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline1.Repositories
{
    public class SeatSaleRepository(AppDbContext db) : ISeatSaleRepository
    {
        private readonly AppDbContext _db = db;

        public async Task<SeatSale?> GetByIdAsync(Guid id)
        {
            return await _db.Set<SeatSale>().FindAsync(id);
        }

        public async Task<IEnumerable<SeatSale>> GetAllAsync()
        {
            return await _db.Set<SeatSale>().ToListAsync();
        }

        public async Task<IEnumerable<SeatSale>> GetActiveDealsAsync(string? type, decimal? priceMin, decimal? priceMax, bool? featured, string? sortBy)
        {
            var query = _db.Set<SeatSale>()
                .Where(ss => ss.IsActive
                    && ss.BookingDeadline > DateOnly.FromDateTime(DateTime.UtcNow)
                    && ss.SeatsLeft > 0);

            if (!string.IsNullOrWhiteSpace(type) && type != "all")
                query = query.Where(ss => ss.Type == type);

            if (priceMin.HasValue)
                query = query.Where(ss => ss.SalePrice >= priceMin.Value);

            if (priceMax.HasValue)
                query = query.Where(ss => ss.SalePrice <= priceMax.Value);

            if (featured.HasValue)
                query = query.Where(ss => ss.Featured == featured.Value);

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(ss => ss.SalePrice),
                "price_desc" => query.OrderByDescending(ss => ss.SalePrice),
                "discount_desc" => query.OrderByDescending(ss => ss.OriginalPrice > 0
                    ? ((ss.OriginalPrice - ss.SalePrice) / ss.OriginalPrice) * 100
                    : 0),
                "seats_asc" => query.OrderBy(ss => ss.SeatsLeft),
                _ => query.OrderByDescending(ss => ss.Featured).ThenByDescending(ss => ss.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<SeatSale> AddAsync(SeatSale seatSale)
        {
            await _db.Set<SeatSale>().AddAsync(seatSale);
            return seatSale;
        }

        public Task UpdateAsync(SeatSale seatSale)
        {
            _db.Set<SeatSale>().Update(seatSale);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
