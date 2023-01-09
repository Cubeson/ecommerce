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
        private IResult TMP()
        {
            return Results.Ok(101);
        }
    }
}
