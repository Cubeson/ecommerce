using Microsoft.Extensions.Primitives;
using Myrmec;
using Server.Utility;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Server.DTO
{
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
}
