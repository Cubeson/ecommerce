using Server.ShopDBContext;
using Shared.DTO;
using Server.Models;
using Server.Utility;
using Microsoft.AspNetCore.Mvc;
using Server.Services.TokenService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Services.SmtpService;

namespace Server.Api;
public sealed class UserApi : IApi
{
    public void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("api/User/Create", CreateUser);
        app.MapPost("api/User/Login", LoginUser);
        app.MapPost("api/User/RequestResetPasswordCode",RequestResetPasswordCode);
        app.MapPost("api/User/ResetPassword", ResetPassword);
        app.MapPost("api/User/RevokeAllSessions", RevokeAllSessions);
        app.MapGet("api/User/IsEmailInUse", IsEmailInUse);
    }
    public IResult ResetPassword([FromBody]ResetPasswordCredentialsDTO credentials, [FromServices] ShopContext context)
    {
        var passRst = context.PasswordResets.Include(pr => pr.User).SingleOrDefault(x => x.ResetID == credentials.ResetId);
        if (passRst == null || passRst.ExpirationDate < DateTime.Now) return Results.BadRequest(new GenericResponseDTO() { Error = 1,Message= "Invalid or expired Reset Code" });
        var user = passRst.User;
        if (!user.SetPassword(credentials.Password)) return Results.BadRequest(new GenericResponseDTO() { Error = 2, Message = "Invalid password" });
        context.ClearUserPasswordResets(user);
        context.SaveChanges();
        return Results.Ok(new GenericResponseDTO() { Message = "Password changed" });
    }
    public IResult RequestResetPasswordCode([FromBody] RequestResetPasswordDTO requestReset, [FromServices] ShopContext context, [FromServices] ISmtpService smtpService)
    {
        var user = context.Users.SingleOrDefault(x => x.Email == requestReset.Email);
        if (user == null) return Results.Empty;

        var passRst = new PasswordReset()
        {
            UserId = user.Id,
            User = user,
            ResetID = Rng.GetRandomStringResetId(8),
            ExpirationDate = DateTime.Now.AddMinutes(Constants.PASSWORD_RESET_LIFETIME_MINUTES)
        };
        context.PasswordResets.Add(passRst);
        context.SaveChanges();
        smtpService.PasswordResetRequested(requestReset,passRst);

        return Results.Empty;
    }

    public bool IsEmailInUse([FromServices] ShopContext shopContext, string email)
    {
        return shopContext.Users.Any(u => u.Email == email);
    }
    public IResult CreateUser([FromBody] UserCreateDTO userDTO, [FromServices] ShopContext shopContext, [FromServices] ISmtpService smtpService)
    {

        if(userDTO.FirstName.Length < 1 || userDTO.LastName.Length < 1)
        {
            return Results.BadRequest(new GenericResponseDTO() {Error = 1, Message = "Invalid name" });
        }
        if (!Shared.Validators.Validators.IsValidPassword(userDTO.Password)){
            return Results.BadRequest(new GenericResponseDTO() { Error = 2, Message = "Invalid password" });
        }

        if(!Shared.Validators.Validators.isValidEmail(userDTO.Email)) {
            return Results.BadRequest(new GenericResponseDTO() { Error = 3, Message = "Invalid email" });
        }

        if(shopContext.Users.Any(u => u.Email.Equals(userDTO.Email)))
        {
            return Results.BadRequest(new GenericResponseDTO() { Error = 4, Message = "Email already in use" });
        }

        var rng = new Random();
        var salt = rng.Next(int.MinValue, int.MaxValue).ToString();
        var hash = StringHasher.HashString(userDTO.Password, salt);
        //var defaultRole = rolesService.GetByName("Default");
        var defaultRole = shopContext.Roles.SingleOrDefault(r => r.Name == Constants.ROLE_DEFAULT);
        var user = new User()
        {
            FirstName = userDTO.FirstName,
            LastName = userDTO.LastName,
            Email = userDTO.Email,
            Password = hash,
            PasswordSalt = salt,
            Role = defaultRole,
            RoleId = defaultRole.Id
        };
        shopContext.Users.Add(user);
        shopContext.SaveChanges();
        smtpService.UserCreated(user);
        return Results.Ok(new GenericResponseDTO() { Message = "Account created" });   
    }
    public IResult LoginUser([FromBody] UserLoginDTO userDTO, [FromServices]ShopContext context, [FromServices]ITokenService tokenService)
    {
        var user = context.Users.SingleOrDefault(u => u.Email.Equals(userDTO.Email));
        if (user == null) return Results.BadRequest("Provided data is incorrect");
        //if (!user.Role.Equals(Constants.RoleDefault)) return Results.BadRequest("Provided data is incorrect");
        var hash = StringHasher.HashString(userDTO.Password, user.PasswordSalt);
        if (!user.Password.Equals(hash)) return Results.BadRequest("Provided data is incorrect");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        context.UserSessions.Add(new UserSession() {
            User = user,
            AuthToken = StringHasher.HashString(accessToken),
            RefreshToken = StringHasher.HashString(refreshToken),
            RefreshTokenExpiryTime = DateTime.Now.AddHours(Constants.REFRESH_TOKEN_EXPIRATION_TIME_HOURS),
        });

        context.SaveChanges();

        return Results.Ok(new TokenModelDTO
        {
            AuthToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresInSeconds = (Constants.TOKEN_EXPIRATION_TIME_MINUTES * 60) - 60
        });;
    }
    public async Task<IResult> RevokeAllSessions([FromBody] UserLoginDTO userDTO, [FromServices] ShopContext shopContext)
    {
        if(userDTO.Email.IsNullOrEmpty() || userDTO.Password.IsNullOrEmpty()) return Results.BadRequest();
        var hashedPass = StringHasher.HashString(userDTO.Password);
        var user = shopContext.Users.Include(u=>u.UserSessions).SingleOrDefault(u => u.Email == userDTO.Email); if(user == null) return Results.BadRequest();
        if (!user.Password.Equals(hashedPass)) return Results.BadRequest();
        var sessions = user.UserSessions;
        await shopContext.UserSessions.ForEachAsync(s => s.IsRevoked= true);
        shopContext.SaveChanges();
        return Results.Ok();
    }

}
