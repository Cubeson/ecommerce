using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Utility;
using Shared.DTO;
using System.Security.Claims;

namespace Server.Api
{
    //public class AdminApi : IApi
    //{
    //    public void Register(WebApplication app)
    //    {
    //        app.MapPost("/api/Admin/Login", Login);
    //    }
    //
    //    public async Task<IResult> Login([FromBody]UserLoginDTO userDTO, [FromServices]ShopContext context, HttpContext httpContext)
    //    {
    //        if (userDTO.Email == null ||userDTO.Password == null) return Results.BadRequest();
    //        var user = context.Users.SingleOrDefault(u=>u.Email == userDTO.Email);
    //        if (user == null) return Results.BadRequest();
    //        if(!user.Role.Equals(Constants.RoleAdmin)) return Results.BadRequest();
    //        if(!user.Password.Equals(StringHasher.HashString(userDTO.Password, user.PasswordSalt))) return Results.BadRequest();
    //
    //        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, "Id","Role");
    //        identity.AddClaim(new Claim("Id", user.Id.ToString()));
    //        identity.AddClaim(new Claim("Role", Constants.RoleAdmin));
    //        var principal = new ClaimsPrincipal(identity);
    //        await httpContext.SignInAsync(
    //            CookieAuthenticationDefaults.AuthenticationScheme,
    //            principal,
    //            new AuthenticationProperties
    //            {
    //                IsPersistent = true,
    //                AllowRefresh = true,
    //                ExpiresUtc = DateTime.UtcNow.AddDays(1)
    //            });
    //        return Results.Ok();
    //    }
    //}
}
