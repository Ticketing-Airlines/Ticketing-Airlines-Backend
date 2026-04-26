using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IUserService
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterUserRequest request);
        Task<LoginResponse?> LoginUserAsync(LoginUserRequest request);
        Task<UserResponse?> GetUserByIdAsync(string id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string id);
    }
}
