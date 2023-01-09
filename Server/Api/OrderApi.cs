using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.ShopDBContext;
using Microsoft.EntityFrameworkCore;
using Server.Services;
using System.Dynamic;
using Newtonsoft.Json;
using Server.Models;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Tokens;

namespace Server.Api;

public class OrderApi : IApi
{
    public void Register(IEndpointRouteBuilder app)
    {

        app.MapPost("api/Order/CreateOrder", CreateOrder);
        app.MapGet("api/Order/GetOrder/{orderId}", GetOrder);
        app.MapDelete("api/Order/CancelOrder", CancelOrder);
        app.MapPost("api/Order/CreatePayPalOrder/{orderId}", CreatePayPalOrder);
        app.MapPost("api/Order/CapturePayPalOrder/{payPalOrderId}", CapturePayPalOrder);
    }
    [Authorize]
    public IResult CreateOrder(HttpContext httpContext, [FromServices] ShopContext shopContext)
    {
        var strId = httpContext.User.FindFirst("Id").Value;
        var id = int.Parse(strId);
        var user = shopContext.Users.SingleOrDefault(u => u.Id == id);

        shopContext.Database.BeginTransaction();

        var existingOrder = shopContext.Orders.SingleOrDefault(o => o.IsCompleted == false && o.UserId == user.Id);
        if(existingOrder != null)
        {
            shopContext.Orders.Remove(existingOrder);
            shopContext.SaveChanges();
        }
    
        Order order = new Order { User = user, UserId = user.Id, };

        var orderItems = shopContext.CartItems.Where(c => c.UserId == user.Id).Include(c=>c.Product).Select(c => new OrderItem
        {
            Order = order,
            OrderId = id,
            Product = c.Product,
            ProductId = c.ProductId,
            ProductName = c.Product.Title,
            ItemPrice = c.Product.Price,
        }).ToList();

        order.OrderItems = orderItems;
        shopContext.OrderItems.AddRange(orderItems);
        shopContext.Orders.Add(order);
        shopContext.SaveChanges();
        shopContext.Database.CommitTransaction();
        return Results.Ok(order.Id);
    }
    public IResult GetOrder([FromServices] ShopContext shopContext, int orderId)
    {
        var order = shopContext.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
        if (order == null) return Results.BadRequest("Order not found");
        var total = order.OrderItems.Aggregate(0, (decimal total, Models.OrderItem next) => total + (next.ItemPrice * next.Quantity));
        return Results.Ok(new
        {
            Total = total,
            Items = order.OrderItems.Select(oi => new {oi.ProductName, oi.ProductId, Price = oi.ItemPrice * oi.Quantity, oi.Quantity }).ToArray(),
            OrderId = order.Id,
        });
    }
    [Authorize]
    public IResult CancelOrder(HttpContext httpContext, [FromServices] ShopContext shopContext)
    {
        var strId = httpContext.User.FindFirst("Id").Value;
        var id = int.Parse(strId);

        var order = shopContext.Orders.SingleOrDefault(o => o.UserId == id);
        if (order == null) return Results.BadRequest("No order to cancel");
        shopContext.Orders.Remove(order);
        shopContext.SaveChanges();
        return Results.Ok();
    }
    public async Task<IResult> CreatePayPalOrder([FromServices] PayPalService payPalService,[FromServices] ShopContext shopContext, int orderId)
    {
        var order = shopContext.Orders.Where(o => o.Id == orderId).Include(o => o.OrderItems).SingleOrDefault();
        if(order == null)
        {
            return Results.BadRequest("No order found");
        }
        if (!order.BrokerOrderId.IsNullOrEmpty())
        {
            var respExisting = await payPalService.GetOrderDetails(order.BrokerOrderId);
            if (!respExisting.IsSuccessStatusCode)
            {
                return Results.BadRequest("No order found");
            }
            string text1 = await respExisting.Content.ReadAsStringAsync();
            dynamic obj1 = JsonConvert.DeserializeObject<ExpandoObject>(text1);
            string status = obj1.status;
            if(status == "COMPLETED")
            {
                return Results.BadRequest("This order is completed");
            }
            return Results.Ok(obj1);
        }
        var resp = await payPalService.CreateOrder(order);
        string text = await resp.Content.ReadAsStringAsync();
        dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(text);
        if (!resp.IsSuccessStatusCode)
        {
            return Results.BadRequest(obj);
        }
        string id = obj.id;
        order.BrokerOrderId = id;
        shopContext.SaveChanges();
        return Results.Ok(obj);
    }
    public async Task<IResult> CapturePayPalOrder([FromServices] ShopContext shopContext,[FromServices] PayPalService payPalService,string payPalOrderId)
    {
        var order = shopContext.Orders.SingleOrDefault(o => o.BrokerOrderId == payPalOrderId);
        if(order == null)
        {
            return Results.BadRequest("Order doesn't exist or was cancelled");
        }

        var resp = await payPalService.CapturePayment(payPalOrderId);
        string text = await resp.Content.ReadAsStringAsync();
        dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(text);
        if (!resp.IsSuccessStatusCode)
        {
            return Results.BadRequest(obj);
        }
        order.IsCompleted = true;
        shopContext.SaveChanges();
        return Results.Ok(obj);
    }

}
