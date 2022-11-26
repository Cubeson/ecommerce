using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Utility;
using System.Data;
namespace Server.AuthRequirements;
public class TokenNotRevokedRequirement : IAuthorizationRequirement
{
}
public class TokenNotRevokedHandler : AuthorizationHandler<TokenNotRevokedRequirement>
{
    private readonly ShopContext _shopContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenNotRevokedHandler([FromServices]ShopContext shopContext, [FromServices]IHttpContextAccessor httpContextAccessor)
    {
        _shopContext = shopContext;
        _httpContextAccessor = httpContextAccessor;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenNotRevokedRequirement requirement)
    {
        var httpRequest = _httpContextAccessor.HttpContext.Request;
        string? token = null;
        try
        {
            token = TokenUtility.GetAuthTokenFromRequest(httpRequest);
        }catch(Exception ex){
            if(ex is NoNullAllowedException || ex is IndexOutOfRangeException)
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }
        if(token == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var userSession = _shopContext.UserSessions.SingleOrDefault(us => us.AuthToken == StringHasher.HashString(token));
        if(userSession == null || userSession.IsRevoked)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        context.Succeed(requirement);
        return Task.CompletedTask;
    
    }
}