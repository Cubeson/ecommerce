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
        }
        public IResult ResetPassword([FromBody]ResetPasswordCredentials credentials, [FromServices] ShopContext context)
        {
            var passRst = context.PasswordResets.Include(pr => pr.User).SingleOrDefault(x => x.ResetID == credentials.ResetId);
            if (passRst == null || passRst.ExpirationDate < DateTime.Now) return Results.BadRequest(new ResetPasswordResponse() { Error = 1,Message= "Invalid or expired Reset Code" });
            var user = passRst.User;
            if (!user.SetPassword(credentials.Password)) return Results.BadRequest(new ResetPasswordResponse() { Error = 2, Message = "Invalid password" });
            context.ClearUserPasswordResets(user);
            context.SaveChanges();
            return Results.Ok(new ResetPasswordResponse() { Message = "Password changed" });
        }
        public IResult RequestResetPasswordCode([FromBody] RequestResetPassword requestReset, [FromServices] ShopContext context)
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

            using (MailMessage mail = new MailMessage())
            {
                var credentials = SmtpCredentials.Get();
                mail.From = new MailAddress(credentials.Email);
                mail.To.Add(requestReset.Email);
                mail.Subject = "Password reset requested";
                mail.Body =
                    "<p>A password reset was requested for an account with your email</p>" +
                    "<p>If you haven't requested a reset, ignore this message</p>" +
                    "<p>If you wish to reset your email, use the code below</p>" +
                    "<p>" + passRst.ResetID + "</p>";
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(credentials.Email, credentials.Password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    return Results.Empty;
                }
            }
        }
        public IResult CreateUser([FromBody] UserCreateDTO userDTO, [FromServices] ShopContext context)
        {

            if(userDTO.FirstName.Length < 1 || userDTO.LastName.Length < 1)
            {
                return Results.BadRequest(new CreateAccountResponse() {Error = 1, Message = "Invalid name" });
            }
            if (!Shared.Validators.Validators.IsValidPassword(userDTO.Password)){
                return Results.BadRequest(new CreateAccountResponse() { Error = 2, Message = "Invalid password" });
            }

            if(!Shared.Validators.Validators.isValidEmail(userDTO.Email)) {
                return Results.BadRequest(new CreateAccountResponse() { Error = 3, Message = "Invalid email" });
            }

            if(context.Users.Any(u => u.Email.Equals(userDTO.Email)))
            {
                return Results.BadRequest(new CreateAccountResponse() { Error = 4, Message = "Email already in use" });
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
        context.SaveChanges();
        Task.Run(() =>
        {
            using (MailMessage mail = new MailMessage())
            {
                var credentials = SmtpCredentials.Get();
                mail.From = new MailAddress(credentials.Email);
                mail.To.Add(user.Email);
                mail.Subject = "Created new account";
                mail.Body = "A new account has been created with this account";
                mail.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(credentials.Email, credentials.Password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        });


        return Results.Ok(new CreateAccountResponse() { Message = "Account created" });
            
        }
        private string ProvidedDataIncorrect = "Provided data is incorrect";

        public IResult LoginUser([FromBody] UserLoginDTO userDTO, [FromServices]ShopContext context, [FromServices]ITokenService tokenService)
        {
            var user = context.Users.SingleOrDefault(u => u.Email.Equals(userDTO.Email));
            if (user == null) return Results.BadRequest(ProvidedDataIncorrect);
            var hash = StringHasher.HashString(userDTO.Password, user.PasswordSalt);
            if (!user.Password.Equals(hash)) return Results.BadRequest(ProvidedDataIncorrect);

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            context.UserSessions.Add(new UserSession() {
                User = user,
                AuthToken = StringHasher.HashString(accessToken),
                RefreshToken = StringHasher.HashString(refreshToken),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(Constants.RefreshTokenExpirationTimeDays)
            });

            //user.RefreshToken = StringHasher.HashString(refreshToken);
            //user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Constants.RefreshTokenExpirationTimeDays);
            context.SaveChanges();

            return Results.Ok(new AuthenticatedResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
        public async Task<IResult> LogoutAllSessions([FromBody] UserLoginDTO userDTO, [FromServices] ShopContext context)
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
