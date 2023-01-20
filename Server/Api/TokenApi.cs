using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.ShopDBContext;
using Shared.DTO;
using Server.Services.TokenService;
using Server.Utility;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Server.Services;

namespace Server.Api;
public class TokenApi : IApi
{
    public void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Token/Refresh",Refresh);
        app.MapPost("api/Token/Logout", Logout);
    }
    public IResult Refresh([FromBody] TokenModelDTO tokenApiModel, [FromServices]ShopContext context, [FromServices]ITokenService tokenService, [FromServices] DateTimeProvider dateTimeProvider)
    {
        return RefreshToken(tokenApiModel, context, tokenService,dateTimeProvider);
    }

    [Authorize]
    public IResult Logout(HttpRequest request ,[FromServices] ShopContext context)
    {
        var token = request.Headers.Authorization[0]?.Substring("Bearer ".Length) ?? null;
        return RevokeToken(context, token);
    }

    ///////////////////////////
    // Static Helper Methods //
    ///////////////////////////

    public static IResult RevokeToken(ShopContext context, string token)
    {
        if (token == null) return Results.BadRequest();
        var userSession = context.UserSessions.SingleOrDefault(s => s.AuthToken == StringHasher.HashString(token));
        if (userSession == null) return Results.BadRequest();
        userSession.IsRevoked = true;
        context.SaveChanges();
        return Results.Ok();
    }

    public static IResult RefreshToken(TokenModelDTO tokenApiModel, ShopContext context, ITokenService tokenService, DateTimeProvider dateTimeProvider)
    {
        if (tokenApiModel.AuthToken.IsNullOrEmpty() || tokenApiModel.RefreshToken.IsNullOrEmpty())
            return Results.BadRequest();

        var userSession = context.UserSessions.Include(us => us.User).SingleOrDefault(us => us.AuthToken == StringHasher.HashString(tokenApiModel.AuthToken));
        if (userSession == null || userSession.RefreshToken != StringHasher.HashString(tokenApiModel.RefreshToken) || userSession.IsRevoked || userSession.RefreshTokenExpiryTime <= dateTimeProvider.Now) return Results.BadRequest();

        var newAuthToken = tokenService.GenerateAccessToken(userSession.User);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        userSession.AuthToken = StringHasher.HashString(newAuthToken);
        userSession.RefreshToken = StringHasher.HashString(newRefreshToken);
        userSession.RefreshTokenExpiryTime = dateTimeProvider.Now.AddHours(Constants.REFRESH_TOKEN_EXPIRATION_TIME_HOURS);
        context.SaveChanges();
        return Results.Ok(new TokenModelDTO()
        {
            AuthToken = newAuthToken,
            RefreshToken = newRefreshToken,
            ExpiresInSeconds = (Constants.TOKEN_EXPIRATION_TIME_MINUTES * 60) - 60
        }) ;
    }

}
