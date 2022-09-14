using Server.Data;
using Server.Endpoints;
using Server.Models;

namespace Server.Extensions
{
    internal static class EndpointsExtension
    {
        internal static void RegisterEndpoints(this WebApplication app)
        {
            ProductEndpoint.Register(app);
            UserEndpoint.Register(app);

        }
    }
}
