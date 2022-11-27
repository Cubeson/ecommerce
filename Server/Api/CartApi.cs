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

            var cart = shopContext.Carts.Where(c => c.User.Id == user.Id).SingleOrDefault();
            if (cart == null) return Results.BadRequest("User doesn't have a cart");

            // Check if user already has this item in their cart

            var cartItem = shopContext.Carts.Where(c => c.User.Id == user.Id).Select(c => c.CartItems.Where(ci => ci.ProductId == cartItemDTO.ProductID))
                .First().SingleOrDefault();

            // If user doesn't have this item in their cart, add it
            if(cartItem == null)
            {
                cartItem = new CartItem() { ProductId = product.Id, Product = product, Quantity = cartItemDTO.Quantity };
                shopContext.CartItems.Add(cartItem);
                cart.CartItems.Add(cartItem);
                shopContext.SaveChanges();
            }
            //otherwise increment item's quantity
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
            var cart = shopContext.Carts.SingleOrDefault(c => c.User.Id == userId);
            if (cart == null) return Results.BadRequest("User doesn't have a cart");
            var ids = newCartDTO.CartItems.Select(ci => ci.ProductID);
            var products = shopContext.Products.Where(p => ids.Any(i => p.Id == i)).ToArray();
            var cartItemsHashSet = products.Join(newCartDTO.CartItems, p => p.Id, c => c.ProductID, (pro, ci) => new CartItem()
            {
                Product = pro,
                ProductId = pro.Id,
                Quantity = ci.Quantity,
            }).ToHashSet();


            //var cartItems = currentCartDTO.CartItems.Select(ci => new CartItem() {
            //    ProductId = ci.ProductID,
            //    //Product = new Product() {Id = ci.ProductID },
            //    Quantity = ci.Quantity,
            //}).ToHashSet();
            //cart.CartItems.Clear();
            var items = cart.CartItems.ToArray();
            shopContext.CartItems.RemoveRange(items);
            shopContext.SaveChanges();
            cart.CartItems = cartItemsHashSet;
            shopContext.SaveChanges();

            return Results.Ok();
        }
        [Authorize(Policy = "Auth", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ICollection<CartItemDTO> GetCurrentCart(HttpContext httpContext, [FromServices] ShopContext shopContext)
        {
            var userId = int.Parse(httpContext.User.FindFirstValue("Id"));
            var user = shopContext.Users.SingleOrDefault(u => u.Id == userId);
            var cartItems = shopContext.Carts.Include(c => c.User).Include(c => c.CartItems)
                .Where(c => c.User.Id == userId)
                .Select(c => c.CartItems)
                .SingleOrDefault();
            return cartItems.Select(c => new CartItemDTO() {ProductID=c.ProductId,Quantity=c.Quantity }).ToArray();
        
        }
    }
}
