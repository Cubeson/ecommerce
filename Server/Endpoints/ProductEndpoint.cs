using Microsoft.AspNetCore.Mvc.Routing;
using Server.Data;
using Server.Models;

namespace Server.Endpoints
{
    public static class ProductEndpoint
    {
        internal static void Register(WebApplication app)
        {
            app.MapGet("/Product/id/{id}", GetProductById);
        }

        public static async ValueTask<Product?> GetProductById(int id, ShopContext context)
        {
            return await context.Products.FindAsync(id);
        }
    }
}
