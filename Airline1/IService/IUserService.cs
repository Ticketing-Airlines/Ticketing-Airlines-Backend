using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IUserService
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterUserRequest request);
        Task<LoginResponse?> LoginUserAsync(LoginUserRequest request);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
}
