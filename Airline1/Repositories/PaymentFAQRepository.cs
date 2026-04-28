using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class PaymentFAQRepository(AppDbContext db) : IPaymentFAQRepository
    {
        public async Task<List<PaymentFAQ>> GetAllActiveAsync()
        {
            return await db.PaymentFAQs
                .AsNoTracking()
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<PaymentFAQ?> GetByIdAsync(int id)
        {
            return await db.PaymentFAQs.FindAsync(id);
        }

        public async Task<PaymentFAQ> AddAsync(PaymentFAQ faq)
        {
            db.PaymentFAQs.Add(faq);
            await db.SaveChangesAsync();
            return faq;
        }

        public async Task UpdateAsync(PaymentFAQ faq)
        {
            db.PaymentFAQs.Update(faq);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(PaymentFAQ faq)
        {
            db.PaymentFAQs.Remove(faq);
            await db.SaveChangesAsync();
        }
    }
}
