using Airline_Ticketing.Data;
using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Airline_Ticketing.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Register a new user
        public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
        {
            // Check if user already exists with this email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            // Hash the password for security
            var hashedPassword = HashPassword(request.Password);

            // Create new user object
            var newUser = new Users
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Email = request.Email,
                Password = hashedPassword,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            // Add to database and save
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Return the user response (without password)
            return new UserResponse
            {
                UserId = newUser.UserID,
                FirstName = newUser.FirstName,
                MiddleName = newUser.MiddleName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Phone = newUser.Phone,
                CreatedAt = newUser.CreatedAt
            };
        }

        // Login user
        public async Task<LoginResponse> LoginAsync(LoginUserRequest request)
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Verify password
            if (!VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Generate token for authentication
            var token = GenerateToken(user);

            // Return login response with token
            return new LoginResponse
            {
                UserId = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = token,
                Message = "Login successful"
            };
        }

        // Get user by ID
        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
            {
                return null;
            }

            return new UserResponse
            {
                UserId = user.UserID,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt
            };
        }

        // Get user by email
        public async Task<UserResponse?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            return new UserResponse
            {
                UserId = user.UserID,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt
            };
        }

        // Get all users
        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserResponse
                {
                    UserId = u.UserID,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        

        // Hash password using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Verify password by comparing hashes
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }

        // Generate a simple token (placeholder - use JWT in production)
        private string GenerateToken(Users user)
        {
            // This is a placeholder token
            // In production, you should use JWT (JSON Web Tokens)
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}