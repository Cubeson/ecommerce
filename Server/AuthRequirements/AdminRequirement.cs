using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ShopDBContext;
using Server.Utility;
using System.Security.Claims;

namespace Server.AuthRequirements;

public class AdminRequirement : IAuthorizationRequirement
{
}
public class AdminHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly ShopContext _shopContext;
    public AdminHandler([FromServices] ShopContext shopContext)
    {
        _shopContext= shopContext;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {

        var user = context.User;
        if (!user.Identity.IsAuthenticated) 
        {
            context.Fail();
            return Task.CompletedTask;
        }
        if (user.FindFirstValue("Role").Equals(Constants.ROLE_ADMIN))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        context.Fail();
        return Task.CompletedTask;

    }
}
