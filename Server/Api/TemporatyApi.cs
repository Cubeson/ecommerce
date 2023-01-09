using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.ShopDBContext;
using Shared.SortOrderDB;
using System.Linq.Dynamic.Core;
namespace Server.Api
{
    public class TemporatyApi : IApi
    {
        public void Register(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/TMP", TMP);
        }
        [Authorize]
        private IResult TMP(HttpContext httpContext, [FromServices] ShopContext shopContext)
        {
            string id = httpContext.User.FindFirst("Id")?.Value ?? "";
            if (id.IsNullOrEmpty()) return Results.BadRequest();

            return Results.Ok($"{id}");
        }
    }
}
