using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.ShopDBContext;
using Server.Models;

namespace Server.Pages.Admin.Products
{
    [Authorize(Policy = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly Server.ShopDBContext.ShopContext _context;
        private readonly IWebHostEnvironment _environment;
        
        public DeleteModel(Server.ShopDBContext.ShopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
      public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                if (!product.Path.IsNullOrEmpty())
                {
                    var path = Path.Combine(_environment.ContentRootPath, "products", product.Path);
                    Directory.Delete(path, true);
                }
                

                Product = product;
                _context.Products.Remove(Product);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
