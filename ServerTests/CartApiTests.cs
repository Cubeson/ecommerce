using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Server.Api;
using Server.Models;
using Server.Services;
using Server.Services.SmtpService;
using Server.Services.TokenService;
using Server.ShopDBContext;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Z.EntityFramework.Plus;

namespace ServerTests;
[Collection("Sequential")]public class CartApiTests : IClassFixture<DatabaseFixture>
{
    ShopContext _shopContext;
    UserApi _userApi = new UserApi();
    CartApi _cartApi= new CartApi();
    User testUser;
    ISmtpService _smtpService = new SmtpTestService();
    static JWTSettings jwtSettings = new JWTSettings() { Audience = "test", Issuer = "test", Key = "TestSecretKey123456789123456789123456789" };
    ITokenService _tokenService = new TokenService(jwtSettings);
    public CartApiTests(DatabaseFixture databaseFixture) {
        _shopContext = databaseFixture.shopContext;
        testUser = databaseFixture.testUser;
    }
    [Fact]public void AddingProductToCart_WhenProductIsValid_ShouldAddToCart()
    {
        int initialCount = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var resp = (Ok<NewQuantityDTO>)_cartApi.AddItem(httpContext, _shopContext, new CartItemDTO { ProductID = 1, Quantity = 1 });
        Assert.Equal(200, resp.StatusCode);
        //int afterCount = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        int afterCount = resp!.Value!.Quantity;
        Assert.NotEqual(initialCount, afterCount);
    }
    [Fact]public void AddingProductToCart_WhenProductDoesntExist_ShouldNotAddToCart()
    {
        int initialCount = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var resp = (BadRequest<string>)_cartApi.AddItem(httpContext, _shopContext, new CartItemDTO { ProductID = 999999, Quantity = 1 });
        Assert.Equal(400,resp.StatusCode);
        int afterCount = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        Assert.Equal(initialCount, afterCount);
    }
    [Fact]public void AddingProductToCart_WhenQuantityIsInvalid_ShouldNotAddToCart()
    {
        int initialCount = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var resp = (BadRequest<string>)_cartApi.AddItem(httpContext, _shopContext, new CartItemDTO { ProductID = 1, Quantity = -1 });
        Assert.Equal(400, resp.StatusCode);
        int afterCount = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        Assert.Equal(initialCount, afterCount);
    }
    [Fact]public void SubstractingProductFromCart_WhenProductIsValid_ShouldSubstractFromToCart()
    {
        _shopContext.CartItems.Delete();
        _shopContext.CartItems.Add(new CartItem { ProductId= 1, Quantity = 1,UserId = testUser.Id });
        _shopContext.SaveChanges();

        int initialQuantity = _shopContext.CartItems.Count(c => c.UserId == testUser.Id && c.ProductId == 1);

        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var resp = (Ok<NewQuantityDTO>)_cartApi.RemoveItem(httpContext, _shopContext, new CartItemDTO { ProductID = 1, Quantity = 1 });
        Assert.Equal(200, resp.StatusCode);
        int afterQuantity = _shopContext.CartItems.Count(c => c.UserId == testUser.Id);
        Assert.True(initialQuantity > afterQuantity);
    }
    [Fact]public void ProductsAddedToCart_ShouldBeTheSameAfterGettingCart()
    {
        var products = new List<CartItemDTO>
        {
            new CartItemDTO { ProductID = 1, Quantity = 1 },
            new CartItemDTO { ProductID = 2, Quantity = 2 },
            new CartItemDTO { ProductID = 3, Quantity = 3 },
        };

        var claims = new[] {
                new Claim("Role","User"),
                new Claim("Id",testUser.Id.ToString()),
            };
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var resp = (Ok)_cartApi.SaveCart(httpContext, _shopContext, products);
        Assert.Equal(200, resp.StatusCode);
        var productsDB = _shopContext.CartItems.Where(c => c.UserId == testUser.Id).ToList();
        Assert.True(productsDB.Where(p => p.ProductId == 2).Single().Quantity == 2);
    }
}
