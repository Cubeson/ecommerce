using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server;
using Server.Api;
using Server.Data;
using Server.Services.TokenService;
using Shared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = 2000000000L); // 2 million bytes
JWTSecretKey.Register(builder.Configuration["SecurityToken:key"], builder.Configuration["SecurityToken:issuer"], builder.Configuration["SecurityToken:audience"]);
SmtpCredentials.Register(builder.Configuration["SmtpClient:email"], builder.Configuration["SmtpClient:password"]);
var secret = JWTSecretKey.Get();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var services = builder.Services;
RegisterServices(services);
RegisterDBContext(services);
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", c => { return Task.Run(() => c.Response.Redirect("/swagger")); });
RegisterApi(app);
SharedClass x = new SharedClass();
app.Run();
void RegisterServices(IServiceCollection services)
{
    services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
        //var secret = SecretKey.GetSecret();
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = secret.Issuer,
            ValidAudience = secret.Audience,
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(secret.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };
    });

    services.AddAuthorization();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecurityScheme,Array.Empty<string>()}
        });


    });
    services.AddTransient<ITokenService,TokenService>();
    
}
void RegisterDBContext(IServiceCollection services)
{
    var connectionString = builder.Configuration.GetConnectionString("localhost");
    services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version())));
} 
void RegisterApi(WebApplication app)
{
    var typeIApi = typeof(IApi);
    var methodInfo = typeIApi.GetMethod("Register");
    var parameters = new object[] { app };
    var types = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(t => t.GetTypes())
        .Where(p => typeIApi.IsAssignableFrom(p) && p.IsClass);
    foreach ( var type in types)
    {
        var instance = Activator.CreateInstance(type);
        methodInfo.Invoke(instance, parameters);
    }

}

public partial class Program { }
//client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);