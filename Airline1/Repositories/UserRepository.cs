using Airline1.Models;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using Airline1.IRepositories;

namespace Airline1.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(int id) =>
            await context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User> AddAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(User user)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await context.Users.ToListAsync();
    }
}
