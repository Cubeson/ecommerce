using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Api;
using Server.AuthRequirements;
using Server.Data;
using Server.Services;
using Server.Services.SmtpService;
using Server.Services.TokenService;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var services = builder.Services;
var configuration = builder.Configuration;
var jwtSettings = RegisterJWTSettings(services, configuration);
var smtpSettings = RegisterSmtpSettings(services, configuration);
AddAuthentication(services,jwtSettings);
AddAuthorization(services);
RegisterServices(services);
RegisterDBContext(services, configuration);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
//app.MapGet("/", c => { return Task.Run(() => c.Response.Redirect("/swagger")); });
RegisterApi(app);

var option = new StaticFileOptions();
FileExtensionContentTypeProvider contentTypeProvider = (FileExtensionContentTypeProvider)option.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings.Add(".data", "application/octet-stream");
option.ContentTypeProvider = contentTypeProvider;
app.UseResponseCaching();
app.UseStaticFiles(option);
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
        o.DefaultAuthenticateScheme = "JWT_OR_COOKIE";
        o.DefaultChallengeScheme = "JWT_OR_COOKIE";
        o.DefaultScheme = "JWT_OR_COOKIE";
    })
        .AddCookie(o =>
        {
            o.LoginPath = "/Admin/Login";
            o.ExpireTimeSpan = TimeSpan.FromDays(1);
        })
        .AddJwtBearer(o =>
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

        }).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", o =>
        {
            o.ForwardDefaultSelector = context =>
            {
                string auth = context.Request.Headers.Authorization;
                if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
                    return JwtBearerDefaults.AuthenticationScheme;
                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });
}
void AddAuthorization(IServiceCollection services)
{
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddTransient<IAuthorizationHandler, TokenNotRevokedHandler>();
    services.AddTransient<IAuthorizationHandler, AdminHandler>();
    services.AddAuthorization(o =>
    {
        o.AddPolicy("Auth", p =>
        {
            p.AddRequirements(new TokenNotRevokedRequirement());
        });
        o.AddPolicy("Admin", p =>
        {
            p.AddRequirements(new AdminRequirement());
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
    services.AddResponseCaching();
}
void RegisterDBContext(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("localhost");
    services.AddDbContext<ShopContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version())));
} 
void RegisterApi(WebApplication app)
{
    var typeIApi = typeof(IApi);
    var method = typeIApi.GetMethod("Register");
    var parameters = new object[] { app };
    var types = Assembly.GetExecutingAssembly().GetExportedTypes()
        .Where(p => typeIApi.IsAssignableFrom(p) && p.IsClass);
    foreach ( var type in types)
    {
        var instance = Activator.CreateInstance(type);
        method.Invoke(instance, parameters);
    }

}
public partial class Program { }