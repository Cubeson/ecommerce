using Server.Data;
using Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

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
        app.MapGet("api/Product/GetModel", GetModel);
        app.MapGet("api/Product/GetCategoryProductsCount",GetCategoryProductsCount);

    } 
    public ProductDTO? GetProduct([FromServices] ShopContext shopContext, int id)
    {
        return shopContext.Products.Where(p => p.Id == id).Select(p => new ProductDTO()
        {
            Id = p.Id,
            Description = p.Description,
            Price = p.Price,
            Title = p.Title,
        }).SingleOrDefault();
    }
    public ProductDTO[] GetProducts([FromServices] ShopContext shopContext, int offset, int count, string category)
    {
        return shopContext.Products.
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
    public int GetCategoryProductsCount([FromServices] ShopContext shopContext , string category)
    {
        return shopContext.Categories.Where(c => c.Name.Equals(category)).Count();

    }
    public CategoryDTO[] GetCategories([FromServices] ShopContext context)
    {
        return context.Categories.Select(c => new CategoryDTO { ID=c.Id, Name=c.Name }).ToArray();
    }
    public IResult GetThumbnail([FromServices] ShopContext shopContext, IWebHostEnvironment environment, int id)
    {
        return GetProductResource(shopContext, id, environment.ContentRootPath, "thumbnail");

    }
    public IResult GetPictures([FromServices] ShopContext shopContext, IWebHostEnvironment environment, int id)
    {
        return GetProductResource( shopContext, id, environment.ContentRootPath, "archive");
    }
    public IResult GetModel([FromServices] ShopContext shopContext, IWebHostEnvironment environment, int id)
    {
        return GetProductResource(shopContext, id, environment.ContentRootPath, "model");
    }
    public static IResult GetProductResource(ShopContext shopContext, int id, string contentRoot, string resourceName)
    {
        var product = shopContext.Products.SingleOrDefault(p => p.Id == id);
        if (product == null) return Results.BadRequest();
        if (product.Path == null) return Results.NotFound();
        var path = Path.Combine(contentRoot, "products", product.Path);
        var fpath = Directory.EnumerateFiles(path, resourceName+".*").SingleOrDefault();
        if (fpath == null) return Results.NotFound();
        var modifiedTime = new FileInfo(fpath).LastWriteTime;
        return Results.File(fpath);
    }

}
