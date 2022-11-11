using Server.Models;
using System.Security.Claims;

namespace Server.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        //string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
