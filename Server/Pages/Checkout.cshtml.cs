using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Server.ShopDBContext;

namespace Server.Pages
{
    //[Authorize]
    public class CheckoutModel : PageModel
    {
        private HttpContext _httpContext;
        private ShopContext _shopContext;
        public int UserId;
        public int OrderId;
        public CheckoutModel(IHttpContextAccessor httpContextAccesor, [FromServices] ShopContext shopContext)
        {
            _httpContext = httpContextAccesor.HttpContext;
            _shopContext = shopContext;
        }

        public void OnGet([FromRoute]int id)
        {
            OrderId = id;
        }
    }
}
