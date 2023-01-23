using Server.Models;
using Server.Utility;

namespace ServerTests;
public class DatabaseFixture : IDisposable
{
    public ShopContext shopContext;
    public User testUser;
    public DatabaseFixture()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShopContext>();
        string connectionString = "server=localhost;user=root;password=1234;database=test;port=3306";
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version()));
        shopContext = new ShopContext(optionsBuilder.Options);
        shopContext.Database.EnsureDeleted();
        shopContext.Database.EnsureCreated();
        
        
        shopContext.Roles.Add(new Server.Models.Role() { Id =1,Name = "Default" });
        testUser = new User {Id=1,Email="TestUser1@gmail.com",FirstName="Test",LastName="Test",RoleId=1, };
        testUser.SetPassword("pass1234");
        shopContext.Users.Add(testUser);
        for (int i = 0; i < 5; i++)
        {
            shopContext.Categories.Add(new Server.Models.Category() { Name = i.ToString() });
        }
        var rng = new Random();
        for (int i = 0; i < 10; i++)
        {
            shopContext.Products.Add(new Server.Models.Product()
            {
                CategoryId = rng.Next(1,6),
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Description = "",
                InStock = 100,
                Path = "",
                Price = rng.Next(20,101),
                Title = i.ToString()
            });;
        }

        shopContext.SaveChanges();
    }
    public void Dispose()
    {
    }
}
