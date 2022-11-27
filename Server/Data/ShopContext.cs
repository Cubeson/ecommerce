using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Server.Models;
using System.Configuration;

namespace Server.Data;

public partial class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options){
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<PasswordReset> PasswordResets { get; set; } = null!;
    //public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<UserSession> UserSessions { get; set; } = null!;
    //public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        builder.Entity<User>().HasMany(u => u.PasswordResets)
            .WithOne(pr => pr.User)
            .HasForeignKey(pr => pr.UserId);

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
