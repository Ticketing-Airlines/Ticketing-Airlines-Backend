using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(string sessionToken);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
