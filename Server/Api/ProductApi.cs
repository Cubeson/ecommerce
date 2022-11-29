using Server.Data;
using Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using static Server.Utility.Constants;
using Microsoft.IdentityModel.Tokens;

namespace Server.Api;
public sealed class ProductApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("api/Product/GetProducts", GetProducts);
        app.MapGet("api/Product/GetProduct", GetProduct);
        app.MapGet("api/Product/GetThumbnail", GetThumbnail);
        app.MapGet("api/Product/GetPictures", GetPictures);
        app.MapGet("api/Product/GetCategories", GetCategories);

    } 
    public ProductDTO? GetProduct([FromServices] ShopContext context, int id)
    {
        return context.Products.Where(p => p.Id == id).Select(p => new ProductDTO()
        {
            Id = p.Id,
            Description = p.Description,
            Price = p.Price,
            Title = p.Title,
        }).SingleOrDefault();
    }
    public ProductDTO[] GetProducts([FromServices] ShopContext context, int offset, int count, string category)
    {
        return context.Products.
            Where(p => p.Category.Name == category)
            .OrderBy(p => p.DateModified)
            .Skip(offset).Take(count)
            .Select(p => new ProductDTO()
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
            }).ToArray();

    }
    [ResponseCache(Duration = 600)]
    public CategoryDTO[] GetCategories([FromServices] ShopContext context)
    {
        return context.Categories.Select(c => new CategoryDTO { ID=c.Id, Name=c.Name }).ToArray();
    }
    [ResponseCache(Duration = 600)]
    public IResult GetThumbnail([FromServices] ShopContext context, IWebHostEnvironment environment, int id)
    {
        return GetProductResource(context, id, environment.ContentRootPath, "thumbnail");

    }
    [ResponseCache(Duration = 600)]
    public IResult GetPictures([FromServices] ShopContext context, IWebHostEnvironment environment, int id)
    {
        return GetProductResource(context, id, environment.ContentRootPath, "archive");
    }

    public static IResult GetProductResource(ShopContext context, int id, string contentRoot, string resourceName)
    {
        var product = context.Products.SingleOrDefault(p => p.Id == id);
        if (product == null) return Results.BadRequest();
        if (product.Path == null) return Results.NotFound();
        var path = Path.Combine(contentRoot, "products", product.Path);
        var fpath = Directory.EnumerateFiles(path, resourceName+".*").SingleOrDefault();
        if (fpath == null) return Results.NotFound();
        return Results.File(fpath);
    }

}
