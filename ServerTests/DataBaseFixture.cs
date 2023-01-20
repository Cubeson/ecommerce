using Server.Models;

namespace ServerTests;
public class DatabaseFixture : IDisposable
{
    public ShopContext shopContext;
    public DatabaseFixture()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShopContext>();
        string connectionString = "server=localhost;user=root;password=1234;database=test;port=3306";
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version()));
        shopContext = new ShopContext(optionsBuilder.Options);
        shopContext.Database.EnsureDeleted();
        shopContext.Database.EnsureCreated();
        shopContext.Roles.Add(new Server.Models.Role() { Name = "Default" });
        //User user = new User { Id = 1,Email = "TestEmail1@gmail.com",FirstName="Test",LastName="Test",RoleId=1,};
        //user.SetPassword("Pass1234");
        //shopContext.Users.Add(user);

        shopContext.SaveChanges();
    }
    public void Dispose()
    {
    }
}
