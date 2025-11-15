using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
