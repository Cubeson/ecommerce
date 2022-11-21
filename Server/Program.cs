using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server;
using Server.Api;
using Server.Data;
using Server.Services.SmtpService;
using Server.Services.TokenService;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = 2000000000L); // 2 million bytes
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTSettings>();
JWTSingleton.Set(jwtSettings);
var smptSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
SmtpSingleton.Set(smptSettings);
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
        var jwtSettings = JWTSingleton.Get();
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };

    });
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddTransient<IAuthorizationHandler,TokenNotRevokedHandler>();
    services.AddAuthorization(o =>
    {
        o.AddPolicy("TokenNotRevoked", p =>
        {
            p.AddRequirements(new TokenNotRevokedRequirement());
        });
    });
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
    services.AddTransient<ISmtpService, SmtpTestService>();

}
void RegisterDBContext(IServiceCollection services)
{
    var connectionString = builder.Configuration.GetConnectionString("localhost");
    services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version())));
} 
void RegisterApi(WebApplication app)
{
    //new UserApi().Register(app);
    //new TokenApi().Register(app);
    //new ProductApi().Register(app);
    //new ImportantResourcesApi().Register(app);

    var typeIApi = typeof(IApi);
    var methodInfo = typeIApi.GetMethod("Register");
    var parameters = new object[] { app };
    var types = Assembly.GetExecutingAssembly().GetExportedTypes()
        .Where(p => typeIApi.IsAssignableFrom(p) && p.IsClass);
    foreach ( var type in types)
    {
        var instance = Activator.CreateInstance(type);
        methodInfo.Invoke(instance, parameters);
    }

}

public partial class Program { }
