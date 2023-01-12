using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Server.Models;
using System.Configuration;

namespace Server.ShopDBContext;

public partial class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options){
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<PasswordReset> PasswordResets { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<UserSession> UserSessions { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
    public void ClearUserPasswordResets(User user)
    {
        PasswordResets.RemoveRange(PasswordResets.Where(x => x.UserId == user.Id));
    }
    public static DbContextOptions<ShopContext> GetInMemoryOptions(string DBName)
    {
        return new DbContextOptionsBuilder<ShopContext>()
            .UseInMemoryDatabase(DBName)
            .Options;
    }
}
