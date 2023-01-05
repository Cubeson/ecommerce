using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.ShopDBContext;
using Server.Models;
using Shared.DTO;
using System.Security.Claims;

namespace Server.Api;
public class CartApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapPost("/api/Cart/AddItem", AddItem);
        app.MapPost("/api/Cart/SaveCart", SaveCart);
        app.MapPost("/api/Cart/GetCart", GetCart);
    }
    [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IResult AddItem(HttpContext httpContext, [FromServices] ShopContext shopContext, CartItemDTO cartItemDTO)
    {
        var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
        var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);
        if (cartItemDTO.ProductID <= 0) return Results.BadRequest("Invalid quantity");

        return AddItemToCart(shopContext, cartItemDTO, user);
    }

    [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IResult SaveCart(HttpContext httpContext, [FromServices] ShopContext shopContext, ICollection<CartItemDTO> newCartDTO)
    {
        var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
        var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);

        return SaveUserCart(shopContext, newCartDTO, user);
    }

    [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ICollection<CartItemDTO> GetCart(HttpContext httpContext, [FromServices] ShopContext shopContext)
    {
        var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
        var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);
        return GetUserCart(shopContext, userId);
    }


                                    ///////////////////////////
                                    // Static Helper Methods //
                                    ///////////////////////////
    public static ICollection<CartItemDTO> GetUserCart(ShopContext shopContext, int userId)
    {
        return shopContext.CartItems.Include(c => c.User)
            .Where(c => c.User.Id == userId).Select(c => new CartItemDTO()
            {
                ProductID = c.ProductId,
                Quantity = c.Quantity,
            }).ToArray();
    }

    public static IResult SaveUserCart(ShopContext shopContext, ICollection<CartItemDTO> newCartDTO, User user)
    {
        if (newCartDTO.GroupBy(c => c.ProductID).Count() != newCartDTO.Count) return Results.BadRequest("Duplicate items in cart");
        var ids = newCartDTO.Select(ci => ci.ProductID);
        var products = shopContext.Products.Where(p => ids.Any(i => p.Id == i)).ToArray();
        var currentCartItems = shopContext.CartItems.Where(c => c.UserId == user.Id).ToArray();
        CartItem[] newCartItems = ConvertCartItemDTOtoCartItem(newCartDTO, user, products);

        shopContext.CartItems.RemoveRange(currentCartItems);
        shopContext.CartItems.AddRange(newCartItems);
        shopContext.SaveChanges();
        return Results.Ok();
    }
    //public static IResult SaveUserCart(ShopContext shopContext, CartDTO newCartDTO, User user)
    //{
    //    if (newCartDTO.CartItems.GroupBy(c => c.ProductID).Count() != newCartDTO.CartItems.Length) return Results.BadRequest("Duplicate items in cart");
    //    var ids = newCartDTO.CartItems.Select(ci => ci.ProductID);
    //    var products = shopContext.Products.Where(p => ids.Any(i => p.Id == i)).ToArray();
    //    var currentCartItems = shopContext.CartItems.Where(c => c.UserId == user.Id).ToArray();
    //    CartItem[] newCartItems = ConvertCartItemDTOtoCartItem(newCartDTO, user, products);
    //
    //    shopContext.CartItems.RemoveRange(currentCartItems);
    //    shopContext.CartItems.AddRange(newCartItems);
    //    shopContext.SaveChanges();
    //    return Results.Ok();
    //}

    public static CartItem[] ConvertCartItemDTOtoCartItem(ICollection<CartItemDTO> newCartDTO, User user, ICollection<Product> products)
    {
        return products.Join(newCartDTO, p => p.Id, c => c.ProductID, (pro, ci) => new CartItem()
        {
            Product = pro,
            ProductId = pro.Id,
            User = user,
            UserId = user.Id,
            Quantity = ci.Quantity
        }).ToArray();
    }

    private static IResult AddItemToCart(ShopContext shopContext, CartItemDTO cartItemDTO, User user)
    {

        var product = shopContext.Products.SingleOrDefault(p => p.Id == cartItemDTO.ProductID);
        if (product == null) return Results.BadRequest("Invalid product");
        var cartItem = shopContext.CartItems.Where(c => c.User.Id == user.Id && c.ProductId == product.Id).SingleOrDefault();
        if (cartItem == null)
        {
            cartItem = new CartItem() { ProductId = product.Id, Product = product, Quantity = cartItemDTO.Quantity, User = user, UserId = user.Id };
            shopContext.CartItems.Add(cartItem);
            shopContext.SaveChanges();
        }
        else
        {
            cartItem.Quantity += cartItemDTO.Quantity;
            shopContext.SaveChanges();
        }
        return Results.Ok();
    }

}
