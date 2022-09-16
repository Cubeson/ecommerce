using Server.Data;
using Server.Endpoints;
using Server.Models;

namespace Server.Extensions
{
    internal static class EndpointsExtension
    {
        internal static void RegisterEndpoints(this WebApplication app)
        {
            app.MapGet("/", c =>{ return Task.Run(() => c.Response.Redirect("/swagger"));});
            app.MapGet("/HelloWorld" , () => "Hello World!");
            ProductEndpoint.Register(app);
            UserEndpoint.Register(app);

        }
    }
}
