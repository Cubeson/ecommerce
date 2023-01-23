using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Server.Api;
using Server.Models;
using System.Security.Claims;
using Moq;
using Server.Services;

namespace ServerTests;
[Collection("Sequential")] public class OrderApiTests : IClassFixture<DatabaseFixture>
{
    ShopContext _shopContext;
    User testUser;
    OrderApi _orderApi = new OrderApi();
    CartApi _cartApi = new CartApi();
    public OrderApiTests(DatabaseFixture databaseFixture) {
        _shopContext = databaseFixture.shopContext;
        testUser = databaseFixture.testUser;
    }
    [Fact] public void CreatingOrder_WithValidCart_OrderContainsTheSameProducts()
    {
        var rng = new Random();
        var products = _shopContext.Products.Take(3).Select(p => 
            new CartItemDTO { ProductID = p.Id, Quantity = rng.Next(1,4) }).ToList();
        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };
         
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _cartApi.SaveCart(httpContext, _shopContext, products);
        var resp = (Ok<OrderDTO>)_orderApi.CreateOrder(httpContext, _shopContext);
        Assert.Equal(products.Count, resp!.Value!.Items!.Count());
        Assert.Contains(resp!.Value!.Items!, p => p.ProductId == products[0].ProductID);
    }
    [Fact] public void CancelingOrder_WhenOrderIsNotCompleted_ShouldCancelOrder()
    {
        var product = _shopContext.Products.First();
        var orderItems = new List<OrderItem>() { new() { ProductId =product.Id, Quantity = 1,ProductName = product.Title } };
        _shopContext.OrderItems.AddRange(orderItems);
        Order order = new Order() { UserId = testUser.Id,OrderItems = orderItems };
        _shopContext.Orders.Add(order);
        _shopContext.SaveChanges();

        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var resp = (Ok)_orderApi.CancelOrder(httpContext, _shopContext, order.Id);
        Assert.False(_shopContext.Orders.Any(o => o.Id == order.Id));
    }
    [Fact]
    public void CancelingOrder_WhenOrderIsCompleted_ShouldNotCancelOrder()
    {
        var product = _shopContext.Products.First();
        var orderItems = new List<OrderItem>() { new() { ProductId = product.Id, Quantity = 1, ProductName = product.Title } };
        _shopContext.OrderItems.AddRange(orderItems);
        Order order = new Order() { UserId = testUser.Id, OrderItems = orderItems,IsCompleted = true };
        _shopContext.Orders.Add(order);
        _shopContext.SaveChanges();

        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var resp = (BadRequest<string>)_orderApi.CancelOrder(httpContext, _shopContext, order.Id);
        Assert.True(_shopContext.Orders.Any(o => o.Id == order.Id));
    }
}
