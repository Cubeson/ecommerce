using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Server.Api
{
    public class ImportantResourcesApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/api/Resources/Important",GetImportantResource);
        }
        [Authorize(Policy = "Auth",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult GetImportantResource(HttpContext httpContext)
        {
            var user = httpContext.User;
            var x = user.FindFirstValue("Id");
            return Results.Ok("This is a very important resource. This token belongs to user with id: " + x);
        } 
    }
}
