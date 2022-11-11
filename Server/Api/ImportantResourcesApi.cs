
using Microsoft.AspNetCore.Authorization;

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
            var name = user.Identity.Name;
            return Results.Ok("This is a very important resource. Hello "+ name + "!");
        } 
    }
}
