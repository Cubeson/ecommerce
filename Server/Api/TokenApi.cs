using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Shared.DTO;
using Server.Services.TokenService;
using Server.Utility;
using Microsoft.IdentityModel.Tokens;

namespace Server.Api
{
    public class TokenApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapPost("api/Token/Refresh",Refresh);
            app.MapPost("api/Token/Revoke",Revoke);
        }
        public IResult Refresh(TokenApiModel tokenApiModel, [FromServices]ShopContext context, [FromServices]ITokenService tokenService)
        {
            if(tokenApiModel.AccessToken.IsNullOrEmpty() || tokenApiModel.RefreshToken.IsNullOrEmpty()) 
                return Results.BadRequest();
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            //refreshToken = PasswordUtility.GenerateHash(refreshToken, "");
            var principal = tokenService.GetPrincipalFromToken(accessToken);
            var email = principal.Identity.Name;
            
            var user = context.Users.SingleOrDefault(u => u.Email == email);
            if(user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) { 
                return Results.BadRequest();
            }
            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshToken = StringHasher.HashString(newRefreshToken);
            context.SaveChanges();
            return Results.Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
        [Authorize]
        public IResult Revoke(HttpContext httpContext ,[FromServices] ShopContext context)
        {
            var email = httpContext.User.Identity?.Name ?? null; 
            if(email == null) return Results.BadRequest();
            var user = context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null) return Results.BadRequest();

            user.RefreshToken = null;
            context.SaveChanges();
            return Results.NoContent();
        }

    }
}
