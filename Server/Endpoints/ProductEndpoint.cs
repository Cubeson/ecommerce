using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Server.Data;
using Server.Models;

namespace Server.Endpoints
{
    public static class ProductEndpoint
    {
        internal static void Register(WebApplication app)
        {
            app.MapGet("/Product/id/{id}",GetProductById);
            app.MapGet("/Download/id/{id}", DownloadProduct);
        }

        public static async ValueTask<Product?> GetProductById(int id, ShopContext context)
        {
            return await context.Products.FindAsync(id);
        }
        public static IResult DownloadProduct(int id,ShopContext context)
        {
            var product = context.Products.First(p => p.Id == id);

            var dir = Directory.GetCurrentDirectory()+"/resources/";

            return Results.File(dir+product.Filename,"","nice.fbx");
        }
    }
}
