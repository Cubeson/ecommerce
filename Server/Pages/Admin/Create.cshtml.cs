using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Data;
using Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Server.Pages.Admin
{

    public class ProdutRazorModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; } = 0;
        public IFormFile ThumbnailImage { get; set; } = default!;
        public IFormFile ArchiveImages { get; set; } = default!;
        [Column(TypeName = "decimal(18, 2)")]
        public string Price { get; set; } = default!;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; } = DateTime.Now;
    }


    [Authorize(Policy = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly Server.Data.ShopContext _shopContext;
        private readonly IWebHostEnvironment _environment;
        public CreateModel(Server.Data.ShopContext shopContext, IWebHostEnvironment environment)
        {
            _shopContext = shopContext;
            _environment = environment;
            
        }
        [BindProperty]
        public ProdutRazorModel ProductModel { get; set; } = new ProdutRazorModel();
        public SelectList Categories { get; set; } = default!;

        public IActionResult OnGet()
        {
            var categoryList = _shopContext.Categories.ToList();
            Categories = new SelectList(categoryList, nameof(Category.Id),nameof(Category.Name));
            return Page();
        }



        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return OnGet();
            }
            var category = _shopContext.Categories.SingleOrDefault(c => c.Id == ProductModel.CategoryId);
            if (category == null) throw new Exception("This category doesn't exist");
            Product Product = new Product()
            {
                CategoryId = ProductModel.CategoryId,
                Category = category,
                DateCreated = ProductModel.DateCreated,
                DateModified = ProductModel.DateModified,
                Description = ProductModel.Description,
                Title = ProductModel.Title,
                Price = decimal.Parse(ProductModel.Price, CultureInfo.InvariantCulture),
                Path = Guid.NewGuid().ToString(),
            };
            var path = Path.Combine(_environment.ContentRootPath, "products", Product.Path);
            _shopContext.Products.Add(Product);
            var taskDB = _shopContext.SaveChangesAsync();
            if (Directory.Exists(path))
            {
                throw new Exception("Path already exists");
            }
            Directory.CreateDirectory(path);
            var ext = Path.GetExtension(ProductModel.ThumbnailImage.FileName);
            using(FileStream stream1 = new FileStream(Path.Combine(path, "thumbnail"+ext), FileMode.Create))
            {
                await ProductModel.ThumbnailImage.CopyToAsync(stream1);
            }
            using (FileStream stream2 = new FileStream(Path.Combine(path, "archive.zip"), FileMode.Create))
            {
                await ProductModel.ArchiveImages.CopyToAsync(stream2);
            }

            await taskDB;

            return RedirectToPage("./Index");
        }
    }
}
