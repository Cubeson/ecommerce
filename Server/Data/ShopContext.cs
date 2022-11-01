using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
        public static DbContextOptions<ShopContext> GetInMemoryOptions(string DBName)
        {
            return new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(DBName)
                .Options;
        }

    }

}
