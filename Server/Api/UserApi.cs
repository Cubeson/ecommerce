using Server.Data;
using Shared.DTO;
using Shared.Validators;
using Server.Models;
using Server.Utility;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Server.Services.TokenService;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Services.SmtpService;

namespace Server.Api
{
    public sealed class UserApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapPost("api/User/Create", CreateUser);
            app.MapPost("api/User/Login", LoginUser);
            app.MapPost("api/User/RequestResetPasswordCode",RequestResetPasswordCode);
            app.MapPost("api/User/ResetPassword", ResetPassword);
            app.MapPost("api/User/RevokeAllSessions", RevokeAllSessions);
        }
        public IResult ResetPassword([FromBody]ResetPasswordCredentialsDTO credentials, [FromServices] ShopContext context)
        {
            var passRst = context.PasswordResets.Include(pr => pr.User).SingleOrDefault(x => x.ResetID == credentials.ResetId);
            if (passRst == null || passRst.ExpirationDate < DateTime.Now) return Results.BadRequest(new ResetPasswordResponseDTO() { Error = 1,Message= "Invalid or expired Reset Code" });
            var user = passRst.User;
            if (!user.SetPassword(credentials.Password)) return Results.BadRequest(new ResetPasswordResponseDTO() { Error = 2, Message = "Invalid password" });
            context.ClearUserPasswordResets(user);
            context.SaveChanges();
            return Results.Ok(new ResetPasswordResponseDTO() { Message = "Password changed" });
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
                ExpirationDate = DateTime.Now.AddMinutes(Constants.PasswordResetLifetimeMinutes)
            };
            context.PasswordResets.Add(passRst);
            context.SaveChanges();
            smtpService.PasswordResetRequested(requestReset,passRst);

            return Results.Empty;
        }
        public IResult CreateUser([FromBody] UserCreateDTO userDTO, [FromServices] ShopContext context, [FromServices] ISmtpService smtpService)
        {

            if(userDTO.FirstName.Length < 1 || userDTO.LastName.Length < 1)
            {
                return Results.BadRequest(new CreateAccountResponseDTO() {Error = 1, Message = "Invalid name" });
            }
            if (!Shared.Validators.Validators.IsValidPassword(userDTO.Password)){
                return Results.BadRequest(new CreateAccountResponseDTO() { Error = 2, Message = "Invalid password" });
            }

            if(!Shared.Validators.Validators.isValidEmail(userDTO.Email)) {
                return Results.BadRequest(new CreateAccountResponseDTO() { Error = 3, Message = "Invalid email" });
            }

            if(context.Users.Any(u => u.Email.Equals(userDTO.Email)))
            {
                return Results.BadRequest(new CreateAccountResponseDTO() { Error = 4, Message = "Email already in use" });
            }

        var rng = new Random();
        var salt = rng.Next(int.MinValue, int.MaxValue).ToString();
        var hash = StringHasher.HashString(userDTO.Password, salt);
        var user = new User()
        {
            FirstName = userDTO.FirstName,
            LastName = userDTO.LastName,
            Email = userDTO.Email,
            Password = hash,
            PasswordSalt = salt
        };
        context.Users.Add(user);
        context.Carts.Add(new Cart()
        {
            User= user,
        });
        context.SaveChanges();
        smtpService.UserCreated(user);
        return Results.Ok(new CreateAccountResponseDTO() { Message = "Account created" });   
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
                RefreshTokenExpiryTime = DateTime.Now.AddHours(Constants.RefreshTokenExpirationTimeHours),
                //RefreshTokenExpiryTime = DateTime.Now.AddDays(Constants.RefreshTokenExpirationTimeDays),
            });

            context.SaveChanges();

            return Results.Ok(new TokenModelDTO
            {
                AuthToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        public async Task<IResult> RevokeAllSessions([FromBody] UserLoginDTO userDTO, [FromServices] ShopContext context)
        {
            if(userDTO.Email.IsNullOrEmpty() || userDTO.Password.IsNullOrEmpty()) return Results.BadRequest();
            var hashedPass = StringHasher.HashString(userDTO.Password);
            var user = context.Users.Include(u=>u.UserSessions).SingleOrDefault(u => u.Email == userDTO.Email); if(user == null) return Results.BadRequest();
            if (!user.Password.Equals(hashedPass)) return Results.BadRequest();
            var sessions = user.UserSessions;
            await context.UserSessions.ForEachAsync(s => s.IsRevoked= true);
            return Results.Ok();
        }

    }
}
