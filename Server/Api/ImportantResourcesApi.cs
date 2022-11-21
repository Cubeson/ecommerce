using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.TokenService;
using Server.Utility;
using System.Security.Claims;

namespace Server.Api
{
    public class ImportantResourcesApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/api/Resources/Important",GetImportantResource);
        }
        [Authorize(Policy = "TokenNotRevoked")]
        public IResult GetImportantResource(HttpContext httpContext)
        {
            var user = httpContext.User;
            var x = user.FindFirstValue("Id");
            return Results.Ok("This is a very important resource. This token belongs to user with id: " + x);
        } 
    }
}
