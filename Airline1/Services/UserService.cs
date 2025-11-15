using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;

namespace Airline1.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<RegisterResponse> RegisterUserAsync(RegisterUserRequest request)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // hashing
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            };

            var created = await userRepository.AddAsync(user);

            return new RegisterResponse { Id = created.Id };
        }

        public async Task<LoginResponse?> LoginUserAsync(LoginUserRequest request)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null) return null;

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!validPassword) return null;

            return new LoginResponse
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role,
                Token = "dummy-token" // TODO: JWT later
            };
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponse
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await userRepository.GetAllAsync();
            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) return null;

            if (!string.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.MiddleName)) user.MiddleName = request.MiddleName;
            if (!string.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
            if(request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth.Value;
            if (!string.IsNullOrEmpty(request.Gender)) user.Gender = request.Gender;
            user.UpdatedAt = DateTime.UtcNow;

            var updated = await userRepository.UpdateAsync(user);

            return new UserResponse
            {
                Id = updated.Id,
                FullName = $"{updated.FirstName} {updated.LastName}",
                Email = updated.Email,
                PhoneNumber = updated.PhoneNumber,
                Role = updated.Role,
                DateOfBirth = updated.DateOfBirth,
                Gender = updated.Gender,
                CreatedAt = updated.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await userRepository.DeleteAsync(user);
            return true;
        }
    }
}
