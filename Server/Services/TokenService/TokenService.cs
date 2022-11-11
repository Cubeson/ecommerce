using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services.TokenService
{
    public class TokenService : ITokenService
    {
        public string GenerateAccessToken(User user)
        {
            var secret = JWTSecretKey.Get();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Key));
            var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Role,"User") // user.Role
            };
            var tokeOptions = new JwtSecurityToken(
                 issuer: secret.Issuer,
                 audience: secret.Audience,
                 claims: claims,
                 expires: DateTime.Now.AddMinutes(Constants.TokenExpirationTimeMinutes),
                 signingCredentials: signinCredentials
             );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
        //public string GenerateAccessToken(IEnumerable<Claim> claims)
        //{
        //    var secret = JWTSecretKey.Get();
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Key));
        //    var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var tokeOptions = new JwtSecurityToken(
        //         issuer: secret.Issuer,
        //         audience: secret.Audience,
        //         claims: claims,
        //         expires: DateTime.Now.AddMinutes(Constants.TokenExpirationTimeMinutes),
        //         signingCredentials: signinCredentials
        //     );
        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //    return tokenString;
        //}

        public string GenerateRefreshToken()
        {
            return Rng.GetRandomString();

        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var secret = JWTSecretKey.Get();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Key)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
