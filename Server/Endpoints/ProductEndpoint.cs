using Server.Data;
using Server.DTO;
using Server.Models;
using static Server.Utility.StaticStrings;
using System.Reflection;
using Myrmec;
using Server.Utility;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.IO.Compression;
using System.Xml.Linq;

namespace Server.Endpoints
{
    public sealed class ProductEndpoint : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/Product/id/{id}", GetProductById);
            app.MapGet("/Download/model/{id}", DownloadProductModel);
            app.MapGet("/Download/thumbnail/{id}", DownloadProductThumbnail);
            app.MapGet("/Download/textures/{id}", DownloadProductTextures);
            app.MapGet("/Product/latest/{limit}", GetProductsLatest);
            //app.MapPost("/Product/add", AddProductAsync).Accepts<IFormFile>("multipart/form-data");
            app.MapPost("/Product/add", AddProduct).Accepts<AddProductRequest>("multipart/form-data");

        }

        public async ValueTask<ProductDTO?> GetProductById(int id, ShopContext context)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return null;
            return new ProductDTO(product);
        }
        public IResult DownloadProductModel(int id, ShopContext context)
        {
            var product = context.Products.First(p => p.Id == id);

            var dir = Directory.GetCurrentDirectory() + "/resources/";

            return Results.File(dir + "/" + product.Name + "/" + FileModelString + "." + product.FileFormat);
        }
        public IResult DownloadProductThumbnail(int id, ShopContext context)
        {
            var product = context.Products.First(p => p.Id == id);

            var dir = Directory.GetCurrentDirectory() + "/resources/";

            return Results.File(dir + "/" + product.Name + "/" + FilethumbnailString + "." + "png");
        }
        public IResult DownloadProductTextures(int id, ShopContext context)
        {
            var product = context.Products.First(p => p.Id == id);

            var dir = Directory.GetCurrentDirectory() + "/resources/";

            return Results.File(dir + "/" + product.Name + "/" + FileArchiveString + "." + "zip");
        }
        public IResult GetProductsLatest(int limit, ShopContext context)
        {
            var products = context.Products.OrderByDescending(p => p.DateCreated).Take(limit).ToArray().Select(p => new ProductDTO(p));
            return Results.Ok(products);
        }
        public class AddProductRequest
        {
            private string? _error;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public IFormFile? FileThumbnail { get; set; }
            public IFormFile? FileModel { get; set; }
            public IFormFile? FileTexturesArchive { get; set; }
            public string? GetError()
            {
                return _error;
            }

            public static async ValueTask<AddProductRequest?> BindAsync(HttpContext context,
                                                           ParameterInfo parameter)
            {
                var form = await context.Request.ReadFormAsync();

                var _name = form["Name"];
                var _description = form["Name"];
                var _price = form["Name"];
                var _fileModel = form.Files["FileModel"];
                var _fileThumbnail = form.Files["FileThumbnail"];
                var _fileTexturesArchive = form.Files["FileTexturesArchive"];
                StringBuilder error = new StringBuilder();
                decimal price;
                /*
                 *  Input validation
                 */
                {
                    if (StringValues.IsNullOrEmpty(_name)) error.AppendLine("Field <Name> is empty");
                    if (StringValues.IsNullOrEmpty(_description)) error.AppendLine("Field <Description> is empty");
                    if (StringValues.IsNullOrEmpty(_price)) error.AppendLine("Field <Price> is empty");
                    if (!decimal.TryParse(form["Price"].ToString(), out price)) error.AppendLine("Field <Price> has incorrect data");
                    if (price < 0) error.AppendLine("Field <Price> has negative value");
                    if (_fileModel == null) { error.AppendLine("Field <FileModel> is empty"); }
                    else
                    {
                        var sniffer = new Sniffer();
                        var supportedFiles = new List<Record>
                        {
                            new Record("fbx","4B 61 79 64 61 72 61 20 46 42 58 20 42 69 6E 61 72 79 20 20"), // Kaydara FBX Binary  
                        };
                        sniffer.Populate(supportedFiles);
                        var results = sniffer.Match(ReadFileUtility.ReadFileHead(_fileModel));
                        if (results.Count == 0) error.AppendLine("Field <FileModel> has incorrect data");
                    }

                    if (_fileThumbnail == null) { error.AppendLine("Field <FileThumbnail> is empty"); }
                    else
                    {
                        var sniffer = new Sniffer();
                        var supportedFiles = new List<Record>
                        {
                            new Record("png","89,50,4e,47,0d,0a,1a,0a"), // ‰PNG  
                        };
                        sniffer.Populate(supportedFiles);
                        var results = sniffer.Match(ReadFileUtility.ReadFileHead(_fileThumbnail));
                        if (results.Count == 0) error.AppendLine("Field <FileThumbnail> has incorrect data");
                    }
                    if (_fileTexturesArchive == null) { error.AppendLine("Field <FileTexturesArchive> is empty"); }
                    else
                    {
                        Stream? stream = _fileTexturesArchive.OpenReadStream();
                        ZipArchive? archive = null;
                        try
                        {
                            archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
                            if (archive.Entries.Count < 1) error.AppendLine("Field <FileTexturesArchive> archive file is empty");
                        }
                        catch (InvalidDataException)
                        {
                            error.AppendLine("Field <FileTexturesArchive> has incorrect data");
                        }
                        finally
                        {
                            stream.Dispose();
                            archive?.Dispose();
                        }

                    }
                }
                if (error.Length > 0) return new AddProductRequest { _error = error.ToString() };

                return new AddProductRequest
                {
                    Name = _name.ToString(),
                    Description = _description.ToString(),
                    Price = price,
                    FileModel = _fileModel,
                    FileThumbnail = _fileThumbnail,
                    FileTexturesArchive = _fileTexturesArchive,
                };

            }
        }
        public async Task<IResult> AddProduct(AddProductRequest request, ShopContext context)
        {
            if(request.GetError() != null) { return Results.BadRequest(request.GetError()); }
 
            Directory.CreateDirectory("Resources/"+request.Name);
            var fileModelUploadStream = request.FileModel?.OpenReadStream();
            var extensionModel = request.FileModel?.FileName.Split(".")[1];
            var fileModelLocal = File.Create("Resources/" + request.Name + "/" + FileModelString + "." + extensionModel);
            await fileModelUploadStream.CopyToAsync(fileModelLocal);
            fileModelUploadStream.Dispose();
            fileModelLocal.Dispose();

            var extensionThumbnail = request.FileThumbnail?.FileName.Split('.')[1];
            var thumbnailLocal = File.Create("Resources/" + request.Name + "/" + FilethumbnailString + "." + extensionThumbnail);
            var thumbnailUploadStream = request.FileThumbnail?.OpenReadStream();
            await thumbnailUploadStream.CopyToAsync(thumbnailLocal);
            thumbnailUploadStream.Dispose();
            thumbnailLocal.Dispose();

            var extensionArchive = request.FileThumbnail?.FileName.Split('.')[1];
            var archiveLocal = File.Create("Resources/" + request.Name + "/" + FileArchiveString + "." + extensionArchive);
            var archiveUploadStream = request.FileTexturesArchive?.OpenReadStream();
            await archiveUploadStream.CopyToAsync(archiveLocal);
            archiveUploadStream.Dispose();
            archiveLocal.Dispose();


            var product = new Product(0,request.Name,request.Description,request.Price , DateTime.Now,DateTime.Now);
            product.FileFormat = extensionModel;
            context.Products.Add(product);
            context.SaveChanges();

            return Results.Ok("Added new product");
        }
    }
}
