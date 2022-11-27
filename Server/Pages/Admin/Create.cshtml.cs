using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Server.Pages.Admin
{
    [Authorize(Policy = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly Server.Data.ShopContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(Server.Data.ShopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        [BindProperty] [Column(TypeName = "decimal(18, 2)")]
        public string Price { get; set; } = default!;
        [BindProperty]
        public IFormFile ThumbnailImage { get; set; } = default!;
        [BindProperty]
        public IFormFile ArchiveImages { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Products == null || Product == null || ThumbnailImage == null)
            {
                return Page();
            }
            Product.Price = decimal.Parse(Price, CultureInfo.InvariantCulture);
            Product.Path = Guid.NewGuid().ToString();
            _context.Products.Add(Product);
            var taskDB = _context.SaveChangesAsync();
            var path = Path.Combine(_environment.ContentRootPath, "products", Product.Path);
            if (Directory.Exists(path))
            {
                throw new Exception("Path already exists");
            }
            Directory.CreateDirectory(path);
            var ext = Path.GetExtension(ThumbnailImage.FileName);
            using(FileStream stream1 = new FileStream(Path.Combine(path, "thumbnail"+ext), FileMode.Create))
            {
                await ThumbnailImage.CopyToAsync(stream1);
            }
            if(ArchiveImages != null)
            {
                using (FileStream stream2 = new FileStream(Path.Combine(path, "archive.zip"), FileMode.Create))
                {
                    await ArchiveImages.CopyToAsync(stream2);
                }
            }


            await taskDB;

            return RedirectToPage("./Index");
        }
    }
}
