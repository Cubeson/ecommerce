using Server.Data;
using Shared.DTO;
using Server.Models;
using static Server.Utility.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Server.Api;
public sealed class ProductApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("api/Product/get", GetProducts);

        //app.MapGet("api/Product/id/{id}", GetProductById);
        //app.MapGet("api/Download/model/{id}", DownloadProductModel);
        //app.MapGet("api/Download/thumbnail/{id}", DownloadProductThumbnail);
        //app.MapGet("api/Download/textures/{id}", DownloadProductTextures);
        //app.MapGet("api/Product/latest/{limit}", GetProductsLatest);
        //app.MapPost("/Product/add", AddProductAsync).Accepts<IFormFile>("multipart/form-data");
        //app.MapPost("api/Product/add", AddProduct).Accepts<AddProductRequest>("multipart/form-data");

    }
    public IResult GetProducts([FromServices] ShopContext context)
    {
        return Results.Ok(context.Products.ToArray());
    }

    

    //public async ValueTask<ProductDTO?> GetProductById(int id, ShopContext context)
    //{
    //    var product = await context.Products.FindAsync(id);
    //    if (product == null) return null;
    //    return new ProductDTO(product);
    //}
    //public IResult DownloadProductModel(int id, ShopContext context)
    //{
    //    var product = context.Products.First(p => p.Id == id);
    //
    //    var dir = Directory.GetCurrentDirectory() + "/resources/";
    //
    //    return Results.File(dir + "/" + product.Name + "/" + FileModelString + "." + product.FileFormat);
    //}
    //public IResult DownloadProductThumbnail(int id, ShopContext context)
    //{
    //    var product = context.Products.First(p => p.Id == id);
    //
    //    var dir = Directory.GetCurrentDirectory() + "/resources/";
    //
    //    return Results.File(dir + "/" + product.Name + "/" + FilethumbnailString + "." + "png");
    //}
    //public IResult DownloadProductTextures(int id, ShopContext context)
    //{
    //    var product = context.Products.First(p => p.Id == id);
    //
    //    var dir = Directory.GetCurrentDirectory() + "/resources/";
    //
    //    return Results.File(dir + "/" + product.Name + "/" + FileArchiveString + "." + "zip");
    //}
    //public IResult GetProductsLatest(int limit, ShopContext context)
    //{
    //    var products = context.Products.OrderByDescending(p => p.DateCreated).Take(limit).ToArray().Select(p => new ProductDTO(p));
    //    return Results.Ok(products);
    //}
    //
    //public async Task<IResult> AddProduct(AddProductRequest request, ShopContext context)
    //{
    //    if(request.GetError() != null) { return Results.BadRequest(request.GetError()); }
    //
    //    Directory.CreateDirectory("Resources/"+request.Name);
    //    var fileModelUploadStream = request.FileModel?.OpenReadStream();
    //    var extensionModel = request.FileModel?.FileName.Split(".")[1];
    //    var fileModelLocal = File.Create("Resources/" + request.Name + "/" + FileModelString + "." + extensionModel);
    //    await fileModelUploadStream.CopyToAsync(fileModelLocal);
    //    fileModelUploadStream.Dispose();
    //    fileModelLocal.Dispose();
    //
    //    var extensionThumbnail = request.FileThumbnail?.FileName.Split('.')[1];
    //    var thumbnailLocal = File.Create("Resources/" + request.Name + "/" + FilethumbnailString + "." + extensionThumbnail);
    //    var thumbnailUploadStream = request.FileThumbnail?.OpenReadStream();
    //    await thumbnailUploadStream.CopyToAsync(thumbnailLocal);
    //    thumbnailUploadStream.Dispose();
    //    thumbnailLocal.Dispose();
    //
    //    var extensionArchive = request.FileTexturesArchive?.FileName.Split('.')[1];
    //    var archiveLocal = File.Create("Resources/" + request.Name + "/" + FileArchiveString + "." + extensionArchive);
    //    var archiveUploadStream = request.FileTexturesArchive?.OpenReadStream();
    //    await archiveUploadStream.CopyToAsync(archiveLocal);
    //    archiveUploadStream.Dispose();
    //    archiveLocal.Dispose();
    //
    //
    //    var product = new Product(0,request.Name,request.Description,request.Price , DateTime.Now,DateTime.Now);
    //    product.FileFormat = extensionModel;
    //    context.Products.Add(product);
    //    context.SaveChanges();
    //
    //    return Results.Ok(request);
    //}
}
