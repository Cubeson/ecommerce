using Microsoft.EntityFrameworkCore;
using Server.Api;
using Server.Data;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = 2000000000L); // 2 million bytes
var services = builder.Services;
RegisterServices(services);
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", c => { return Task.Run(() => c.Response.Redirect("/swagger")); });
app.MapGet("/Hello", () => { return "Hello World!"; });
RegisterApi();
app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    var connectionString = builder.Configuration.GetConnectionString("localhost");
    services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version())));
}
void RegisterApi()
{
    new ProductApi().Register(app);
    new UserApi().Register(app);
}

public partial class Program { }