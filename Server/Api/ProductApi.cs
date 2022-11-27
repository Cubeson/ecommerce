using Server.Data;
using Shared.DTO;
using static Server.Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Myrmec.Mime;

namespace Server.Api;
public sealed class ProductApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("api/Product/GetProducts", GetProducts);
        app.MapGet("api/Product/GetThumbnail", GetThumbnail);

    }
    public ProductDTO[] GetProducts([FromServices] ShopContext context, int offset, int count)
    {

        return context.Products.Skip(offset).Take(count)
            .Select(p => new ProductDTO()
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
            }).ToArray();
    }

    public IResult GetThumbnail([FromServices] ShopContext context, IWebHostEnvironment environment, int id)
    {
        var product = context.Products.SingleOrDefault(p=> p.Id == id);
        if(product == null) return Results.BadRequest();
        if(product.Path == null) return Results.NotFound();
        var path = Path.Combine(environment.ContentRootPath, "products", product.Path);
        var fpath = Directory.EnumerateFiles(path, "thumbnail.*").SingleOrDefault();
        if(fpath == null) return Results.NotFound();
        return Results.File(fpath);

    }
 
}
