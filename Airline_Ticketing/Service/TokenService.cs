using Airline_Ticketing.IServices;
using Airline_Ticketing.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Airline_Ticketing.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateAccessToken(Users user)
        {
            var claims = new List<Claim> { 
            
             new Claim(ClaimTypes.Name, user.Name),

             new Claim(ClaimTypes.Email, user.Email),

             new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())


            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.Now.AddMinutes(15);

            var token = new JwtSecurityToken(
               issuer: _config["Jwt:Issuer"],
               audience: _config["Jwt:Audience"],
               claims: claims,
               expires: expiry,
               signingCredentials: creds
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        }
}
