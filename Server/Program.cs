using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;
using Server.DTO;
using Server.Extensions;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("localhost");
services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version() )));
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.RegisterEndpoints();
app.Run();
