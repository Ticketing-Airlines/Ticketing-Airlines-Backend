using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IPaymentFAQRepository
    {
        Task<List<PaymentFAQ>> GetAllActiveAsync();
        Task<PaymentFAQ?> GetByIdAsync(int id);
        Task<PaymentFAQ> AddAsync(PaymentFAQ faq);
        Task UpdateAsync(PaymentFAQ faq);
        Task DeleteAsync(PaymentFAQ faq);
    }
}
