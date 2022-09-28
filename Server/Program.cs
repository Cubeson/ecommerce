using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Server.Data;
using Server.Endpoints;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = 2000000000L); // 2 million bytes
var services = builder.Services;
RegisterServices(services);
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", c => { return Task.Run(() => c.Response.Redirect("/swagger")); });
app.MapGet("/Hello", () => { return "Hello World!"; });
var apis = app.Services.GetServices<IApi>();
foreach(var api in apis)
{
    api.Register(app);
}

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddTransient<IApi, ProductEndpoint>();
    services.AddTransient<IApi, UserEndpoint>();
    var connectionString = builder.Configuration.GetConnectionString("localhost");
    services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version())));
}

public partial class Program { }