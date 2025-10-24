using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Airline1.IRepositories;
using Airline1.Models;
using BCrypt.Net;

namespace Airline1.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;

        public AuthService(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _authRepo.GetByEmailAsync(request.Email);
            if (user == null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isValid) return null;

            user.SessionToken = Guid.NewGuid().ToString("N");
            user.SessionTokenExpiry = DateTime.UtcNow.AddHours(2);
            user.LastLogin = DateTime.UtcNow;

            await _authRepo.UpdateAsync(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Role = user.Role,
                SessionToken = user.SessionToken,
                SessionExpiry = user.SessionTokenExpiry
            };
        }

        public async Task<bool> LogoutAsync(string sessionToken)
        {
            var user = await _authRepo.GetBySessionTokenAsync(sessionToken);
            if (user == null) return false;

            user.SessionToken = null;
            user.SessionTokenExpiry = null;
            await _authRepo.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _authRepo.GetByEmailAsync(request.Email);
            if (user == null) return false;

            user.ResetToken = Guid.NewGuid().ToString("N");
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _authRepo.UpdateAsync(user);

            // Normally you'd send an email here.
            Console.WriteLine($"Reset token for {user.Email}: {user.ResetToken}");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _authRepo.GetBySessionTokenAsync(request.ResetToken);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _authRepo.UpdateAsync(user);
            return true;
        }
    }
}
