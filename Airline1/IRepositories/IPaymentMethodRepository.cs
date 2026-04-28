using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IPaymentMethodRepository
    {
        Task<List<PaymentMethod>> GetAllActiveAsync();
        Task<List<PaymentMethod>> GetActiveByCategoryAsync(string category);
        Task<List<PaymentMethod>> GetActiveFeaturedAsync();
        Task<PaymentMethod?> GetByIdAsync(string id);
        Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod);
        Task UpdateAsync(PaymentMethod paymentMethod);
        Task DeleteAsync(PaymentMethod paymentMethod);
    }
}
