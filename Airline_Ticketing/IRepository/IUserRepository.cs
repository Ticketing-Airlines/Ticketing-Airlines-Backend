using Airline_Ticketing.Model;

namespace Airline_Ticketing.IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetAllAsync();
        Task<Users?> GetByIdAsync(int id);

        Task<Users?> GetByEmailAsync(string email);

        Task<Users> AddAsync(Users user);

        Task<Users> UpdateAsync (Users user);

        Task<bool> DeleteAsync(int id);

        Task<bool> EmailExistsAsync(string email);



    }
}
