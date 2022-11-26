using Server.Data;
using Shared.DTO;
using static Server.Utility.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Server.Api;
public sealed class ProductApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("api/Product/get", GetProducts);

    }
    public ProductDTO[] GetProducts([FromServices] ShopContext context, int offset, int count)
    {

        return context.Products.Skip(offset).Take(count)
            .Select(p => new ProductDTO()
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
            }).ToArray();
    }

 
}
