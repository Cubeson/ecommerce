using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Shared.DTO;
using Server.Services.TokenService;
using Server.Utility;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Server.Api
{
    public class TokenApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapPost("api/Token/Refresh",Refresh);
            app.MapPost("api/Token/Revoke",Revoke);
        }
        public IResult Refresh([FromBody] TokenModelDTO tokenApiModel, [FromServices]ShopContext context, [FromServices]ITokenService tokenService)
        {
            if(tokenApiModel.AuthToken.IsNullOrEmpty() || tokenApiModel.RefreshToken.IsNullOrEmpty()) 
                return Results.BadRequest();
            string? accessToken = tokenApiModel.AuthToken;
            string? refreshToken = tokenApiModel.RefreshToken;
            if(accessToken.IsNullOrEmpty() || refreshToken.IsNullOrEmpty()) return Results.BadRequest();

            var userSession = context.UserSessions.Include(us => us.User).SingleOrDefault(us => us.AuthToken == StringHasher.HashString(accessToken));
            if (userSession == null || userSession.RefreshToken != StringHasher.HashString(refreshToken) || userSession.IsRevoked || userSession.RefreshTokenExpiryTime <= DateTime.Now) return Results.BadRequest();

            var newAuthToken = tokenService.GenerateAccessToken(userSession.User);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            userSession.AuthToken = StringHasher.HashString(newAuthToken);
            userSession.RefreshToken = StringHasher.HashString(newRefreshToken);
            userSession.RefreshTokenExpiryTime = DateTime.Now.AddHours(Constants.RefreshTokenExpirationTimeHours);
            context.SaveChanges();
            return Results.Ok(new TokenModelDTO()
            {
                AuthToken = newAuthToken,
                RefreshToken = newRefreshToken,
            });
        }
        [Authorize]
        public IResult Revoke(HttpRequest request ,[FromServices] ShopContext context)
        {
            var token = request.Headers.Authorization[0]?.Substring("Bearer ".Length) ?? null;
            if(token == null) return Results.BadRequest();
            var userSession  = context.UserSessions.SingleOrDefault(s => s.AuthToken == StringHasher.HashString(token));
            if (userSession == null) return Results.BadRequest();
            userSession.IsRevoked = true;
            context.SaveChanges();
            return Results.NoContent();

            //var userSession =  context.UserSessions.SingleOrDefault(s => s.AuthToken== token);
            //if(userSession == null) return Results.NoContent();
            //userSession.IsRevoked = true;
            //var email = httpContext.User.Identity?.Name ?? null; 
            //if(email == null) return Results.BadRequest();
            //var user = context.Users.SingleOrDefault(u => u.Email == email);
            //if (user == null) return Results.BadRequest();
            //
            //user.RefreshToken = null;
            //context.SaveChanges();
            //return Results.NoContent();
        }

    }
}
