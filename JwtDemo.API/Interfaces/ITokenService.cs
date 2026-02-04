using JwtDemo.API.Models;

namespace JwtDemo.API.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        RefreshToken GenerateRefreshToken();
    }
}
