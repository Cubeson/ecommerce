using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Shared.DTO;
using System.Security.Claims;

namespace Server.Api
{
    public class CartApi : IApi
    {


        public void Register(WebApplication app)
        {
            app.MapPost("/api/Cart/AddItem", AddItem);
            app.MapPost("/api/Cart/SaveCurrentCart", SaveCurrentCart);
            app.MapPost("/api/Cart/GetCurrentCart", GetCurrentCart);
        }
        [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult AddItem(HttpContext httpContext, [FromServices] ShopContext shopContext, CartItemDTO cartItemDTO)
        {
            if (cartItemDTO.ProductID <= 0) return Results.BadRequest("Invalid quantity");
            var product = shopContext.Products.SingleOrDefault(p => p.Id == cartItemDTO.ProductID);
            if (product == null) return Results.BadRequest("Invalid product");

            var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
            var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);

            var cartItem = shopContext.CartItems.Where(c => c.User.Id == user.Id && c.ProductId == product.Id).SingleOrDefault();

            if(cartItem == null)
            {
                cartItem = new CartItem() { ProductId = product.Id, Product = product, Quantity = cartItemDTO.Quantity,User = user,UserId = userId };
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
        [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult SaveCurrentCart(HttpContext httpContext, [FromServices] ShopContext shopContext, CartDTO newCartDTO)
        {
            var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
            var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);

            if (newCartDTO.CartItems.GroupBy(c => c.ProductID).Count() != newCartDTO.CartItems.Length) return Results.BadRequest("Duplicate items in cart");

            var ids = newCartDTO.CartItems.Select(ci => ci.ProductID);
            var products = shopContext.Products.Where(p => ids.Any(i => p.Id == i)).ToArray();

            var currentCartItems = shopContext.CartItems.Where(c => c.UserId == userId).ToArray();
            var newCartItems = products.Join(newCartDTO.CartItems, p => p.Id, c => c.ProductID, (pro, ci) => new CartItem()
            {
                Product = pro,
                ProductId = pro.Id,
                User = user,
                UserId = userId,
                Quantity = ci.Quantity
            }).ToArray();

            shopContext.CartItems.RemoveRange(currentCartItems);
            shopContext.CartItems.AddRange(newCartItems);
            shopContext.SaveChanges();
            return Results.Ok();
        }
        [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ICollection<CartItemDTO> GetCurrentCart(HttpContext httpContext, [FromServices] ShopContext shopContext)
        {
            var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
            var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);
            var cartItems = shopContext.CartItems.Include(c => c.User)
                .Where(c => c.User.Id == userId);
            return cartItems.Select(c => new CartItemDTO() {
                ProductID=c.ProductId,
                Quantity=c.Quantity,
            }).ToArray();
        }

    }
}
