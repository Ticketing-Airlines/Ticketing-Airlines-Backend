using Airline_Ticketing.Model;

namespace Airline_Ticketing.IServices
{
    public interface ITokenService
    {

        string CreateAccessToken(Users user);

        string CreateRefreshToken();

    }
}
