using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;

namespace Airline_Ticketing.IServices
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(RegisterUserRequest request);

        Task<LoginResponse> LoginAsync(LoginUserRequest request);

        Task<UserResponse?> GetUserByIdAsync(int id);

        Task<UserResponse?> GetUserByEmailAsync(string email);

        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    }
}
