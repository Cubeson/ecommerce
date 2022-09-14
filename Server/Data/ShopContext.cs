using Microsoft.EntityFrameworkCore;
using Server.Models;
using System.Configuration;

namespace Server.Data
{
    public sealed class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options){
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        

    }
}
