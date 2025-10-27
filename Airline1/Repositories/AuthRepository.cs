using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class AuthRepository(AppDbContext db) : IAuthRepository
    {

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetBySessionTokenAsync(string token)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.SessionToken == token);
        }

        public async Task UpdateAsync(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await db.SaveChangesAsync();
    }
}
