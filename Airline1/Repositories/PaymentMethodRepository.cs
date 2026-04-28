using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class PaymentMethodRepository(AppDbContext db) : IPaymentMethodRepository
    {
        public async Task<List<PaymentMethod>> GetAllActiveAsync()
        {
            return await db.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.IsActive)
                .OrderBy(pm => pm.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetActiveByCategoryAsync(string category)
        {
            return await db.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.IsActive && pm.Category == category)
                .OrderBy(pm => pm.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetActiveFeaturedAsync()
        {
            return await db.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.IsActive && pm.Featured)
                .OrderBy(pm => pm.DisplayOrder)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(string id)
        {
            return await db.PaymentMethods.FindAsync(id);
        }

        public async Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod)
        {
            db.PaymentMethods.Add(paymentMethod);
            await db.SaveChangesAsync();
            return paymentMethod;
        }

        public async Task UpdateAsync(PaymentMethod paymentMethod)
        {
            db.PaymentMethods.Update(paymentMethod);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(PaymentMethod paymentMethod)
        {
            db.PaymentMethods.Remove(paymentMethod);
            await db.SaveChangesAsync();
        }
    }
}
