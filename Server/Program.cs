using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server;
using Server.Api;
using Server.Data;
using Server.Models;
using Server.Services.SmtpService;
using Server.Services.TokenService;
using Shared.DTO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = 2000000000L); // 2 million bytes
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var services = builder.Services;
var jwtSettings = RegisterJWTSettings(services,builder.Configuration);
var smtpSettings = RegisterSmtpSettings(services,builder.Configuration);
AddAuthentication(services,jwtSettings);
AddAuthorization(services);
RegisterServices(services);
RegisterDBContext(services);
RegisterAutoMapper(services);
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
//app.MapGet("/", c => { return Task.Run(() => c.Response.Redirect("/swagger")); });
RegisterApi(app);
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")),
    RequestPath = "",
    EnableDirectoryBrowsing = true
});
app.MapRazorPages();
app.Run();
SmtpSettings RegisterSmtpSettings(IServiceCollection services, IConfiguration configuration)
{

    var smptSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
    if (smptSettings == null) throw new Exception();
    services.AddSingleton<SmtpSettings>(smptSettings);
    return smptSettings;
}
JWTSettings RegisterJWTSettings(IServiceCollection services, IConfiguration configuration)
{
    var jwtSettings = configuration.GetSection("JwtSettings").Get<JWTSettings>();
    if (jwtSettings == null) throw new Exception();
    services.AddSingleton<JWTSettings>(jwtSettings);
    return jwtSettings;
}
void AddAuthentication(IServiceCollection services, JWTSettings jwtSettings)
{
    services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
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
}
void AddAuthorization(IServiceCollection services)
{
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddTransient<IAuthorizationHandler, TokenNotRevokedHandler>();
    services.AddAuthorization(o =>
    {
        o.AddPolicy("TokenNotRevoked", p =>
        {
            p.AddRequirements(new TokenNotRevokedRequirement());
        });
    });
}
void RegisterServices(IServiceCollection services)
{
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
    services.AddTransient<ISmtpService, SmtpService>();
    services.AddRazorPages();
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
    var types = Assembly.GetExecutingAssembly().GetExportedTypes()
        .Where(p => typeIApi.IsAssignableFrom(p) && p.IsClass);
    foreach ( var type in types)
    {
        var instance = Activator.CreateInstance(type);
        methodInfo.Invoke(instance, parameters);
    }

}

IMapper RegisterAutoMapper(IServiceCollection services)
{
    var conf = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Product, ProductDTO>();
    });
#if DEBUG
    conf.AssertConfigurationIsValid();
#endif
    var mapper = conf.CreateMapper();
    services.AddSingleton<IMapper>(mapper);
    return mapper;
}



public partial class Program { }
