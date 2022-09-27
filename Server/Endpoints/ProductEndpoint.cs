using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Server.Data;
using Server.DTO;
using Server.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Extensions;
using static Server.Utility.StaticStrings;
namespace Server.Endpoints
{
    public static class ProductEndpoint
    {
        internal static void Register(WebApplication app)
        {
            app.MapGet("/Product/id/{id}",GetProductById);
            app.MapGet("/Download/model/{id}", DownloadProductModel);
            app.MapGet("/Download/thumbnail/{id}", DownloadProductThumbnail);
            app.MapGet("/Product/latest/{limit}", GetProductsLatest);
            app.MapPost("/Product/add",AddProductAsync).Accepts<IFormFile>("multipart/form-data");
        }

        public static async ValueTask<ProductDTO?> GetProductById(int id, ShopContext context)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return null;
            return new ProductDTO(product);
        }
        public static IResult DownloadProductModel(int id,ShopContext context)
        {
            var product = context.Products.First(p => p.Id == id);

            var dir = Directory.GetCurrentDirectory()+"/resources/";

            return Results.File(dir+"/"+product.Name+"/"+FileModelString+"."+product.FileFormat);
        }
        public static IResult DownloadProductThumbnail(int id, ShopContext context)
        {
            var product = context.Products.First(p => p.Id == id);

            var dir = Directory.GetCurrentDirectory() + "/resources/";

            return Results.File(dir + "/" + product.Name + "/" + FilethumbnailString + "." + "png");
        }
        public static IResult GetProductsLatest(int limit, ShopContext context)
        {
            var products = context.Products.OrderByDescending(p => p.DateCreated).Take(limit).ToArray().Select(p => new ProductDTO(p));
            return Results.Ok(products);
        }

        public static async Task<IResult> AddProductAsync(IFormFile fileModelUpload,IFormFile thumbnailUpload,
           string Name,
           string Description,
           decimal Price,
           ShopContext context)
       {
            if (!thumbnailUpload.FileName.Split(".")[1].Equals("png")){
                return Results.Ok(new Utility.ResponseBody("1", "Invalid thumbnail format"));
            }
            if(context.Products.Any(p => p.Name.Equals(Name))){
                return Results.Ok(new Utility.ResponseBody("1","Product with this name already exists"));
            }
            

            Directory.CreateDirectory("Resources/"+ Name);

            var extensionModel = fileModelUpload.FileName.Split(".")[1];
            var fileModelLocal = File.Create("Resources/" + Name + "/" + FileModelString +"."+ extensionModel);
            var fileModelUploadStream = fileModelUpload.OpenReadStream();
            await fileModelUploadStream.CopyToAsync(fileModelLocal);
            //await fileModelLocal.WriteAsync(await fileModelUpload.GetBytes());
            fileModelUploadStream.Dispose();
            fileModelLocal.Dispose();
            
            var extensionThumbnail = thumbnailUpload.FileName.Split('.')[1];
            var thumbnailLocal = File.Create("Resources/" + Name + "/" + FilethumbnailString + "." + extensionThumbnail);
            var thumbnailUploadStream = thumbnailUpload.OpenReadStream();
            await thumbnailUploadStream.CopyToAsync(thumbnailLocal);
            //await thumbnailLocal.WriteAsync(await thumbnailUpload.GetBytes());
            thumbnailUploadStream.Dispose();
            thumbnailLocal.Dispose();
       
            Product product = new Product(0, Name, Description, Price, DateTime.Now, DateTime.Now);
            product.FileFormat = extensionModel;
            context.Products.Add(product);
            context.SaveChanges();  
            return Results.Ok(new Utility.ResponseBody("0", "Added new product"));
       
       }
    }
}
