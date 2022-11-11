using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Utility;
using System.Data;

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
        var token = httpRequest.Headers.Authorization[0]?.Substring("Bearer ".Length) ?? throw new NoNullAllowedException("Null token");
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