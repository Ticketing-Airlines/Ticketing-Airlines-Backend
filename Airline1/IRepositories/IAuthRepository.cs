using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetBySessionTokenAsync(string token);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}
