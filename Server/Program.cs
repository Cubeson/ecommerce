using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Extensions;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("localhost");
services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version() )));
//services.AddDbContext<ShopContext>(options => options.UseInMemoryDatabase("Test"));
//services.AddSingleton(ShopContext.CreateInMemoryDBContext("mem"));
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.RegisterEndpoints();

app.Run();
