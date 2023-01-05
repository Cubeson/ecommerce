using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Server.ShopDBContext;
using Server.Models;

namespace Server.Pages.Admin.Products
{
    [Authorize(Policy = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly Server.ShopDBContext.ShopContext _context;

        public IndexModel(Server.ShopDBContext.ShopContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            Logger = loggerFactory.CreateLogger("nice");
        }

        public IList<Product> Product { get;set; } = default!;
        public ILogger Logger { get; set; }
        public int i = 0;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products.Include(p=>p.Category).ToListAsync();
            }
        }
    }
}
