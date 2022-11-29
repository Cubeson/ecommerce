using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Utility;
using Shared.DTO;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Server.Pages.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        private readonly ShopContext _shopContext;
        public LoginModel(ShopContext shopContext)
        {
            _shopContext= shopContext;
        }
        public async Task<IActionResult> OnPost()
        {
            var ok = await Login(Credential, _shopContext, this.HttpContext);
            if (ok)
            {
                return this.Redirect("/Admin");
            }
            return this.Page();
        }
        public async Task<bool> Login(Credential credential, ShopContext shopContext, HttpContext httpContext)
        {
            if (credential.Email == null || credential.Password == null) return false;
            var user = shopContext.Users.SingleOrDefault(u => u.Email == credential.Email);
            if (user == null) return false;
            var roleAdmin = shopContext.Roles.SingleOrDefault(r => r.Name == Constants.ROLE_ADMIN);
            if (!user.Role.Name.Equals(roleAdmin.Name)) return false;
            if (!user.Password.Equals(StringHasher.HashString(credential.Password, user.PasswordSalt))) return false;

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, "Id", "Role");
            identity.AddClaim(new Claim("Id", user.Id.ToString()));
            identity.AddClaim(new Claim("Role", Constants.ROLE_ADMIN));
            var principal = new ClaimsPrincipal(identity);
            //this.SignIn(principal, new AuthenticationProperties
            //{
            //    IsPersistent = true,
            //    AllowRefresh = true,
            //    ExpiresUtc = DateTime.UtcNow.AddDays(1),
            //},CookieAuthenticationDefaults.AuthenticationScheme);
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });
            return true;
        }

    }
    public class Credential
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
