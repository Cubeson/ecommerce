namespace ServerTests
{
    public static class TestWebApplicationFactory
    {
        public static WebApplicationFactory<Program> Create(string name)
        {
            var app = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<ShopContext>>();
                    services.AddDbContext<ShopContext>(options => options.UseInMemoryDatabase(name));
                });
            });
            return app;
        }
    }
}
