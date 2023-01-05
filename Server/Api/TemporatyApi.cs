using Microsoft.AspNetCore.Mvc;
using Server.ShopDBContext;
using Shared.SortOrderDB;
using System.Linq.Dynamic.Core;
namespace Server.Api
{
    public class TemporatyApi : IApi
    {
        public void Register(WebApplication app)
        {
            //app.MapGet("/TMP", TMP);
        }
        //private IResult TMP([FromServices]ShopContext shopContext, SortOrderDB sortOrder)
        //{
        //    var data = new SortOrderDBData(sortOrder);
        //    var query = shopContext.Products
        //                    .Where(p => p.Category.Name == "Laptop")
        //                    .OrderBy($"{data.Row} {data.Direction}");
        //    return Results.Ok(query.ToArray());
        //}
    }
}
