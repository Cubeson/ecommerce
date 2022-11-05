using Server.Data;
using Server.DTO;
using System.Security.Cryptography;
using Server.Models;
using Server.Utility;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Server.Services.TokenService;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Server.Api
{
    public sealed class UserApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapPost("api/User/Create", CreateUser);
            app.MapPost("api/User/Login", LoginUser);
            app.MapPost("api/User/RequestResetPassword",RequestResetPassword);
            app.MapPost("api/User/ResetPassword", ResetPassword);
        }
        public IResult ResetPassword([FromBody]PasswordResetRequest passRstReq, [FromServices] ShopContext context)
        {
            var passRst = context.PasswordResets.Include(pr => pr.User).SingleOrDefault(x => x.ResetID == passRstReq.ResetId);
            if (passRst == null || passRst.ExpirationDate < DateTime.Now) return Results.BadRequest("Invalid Id or expired request");
            var user = passRst.User;
            if (!user.SetPassword(passRstReq.Password)) return Results.BadRequest("Invalid password");
            context.ClearUserPasswordResets(user);
            context.SaveChanges();
            return Results.Ok("Password changed");
        }
        public IResult RequestResetPassword(string email, [FromServices] ShopContext context)
        {
            var user = context.Users.SingleOrDefault(x => x.Email == email);
            if (user == null) return Results.Empty;

            var passRst = new PasswordReset()
            {
                UserId = user.Id,
                User = user,
                ResetID = Rng.GetRandomStringResetId(16),
                ExpirationDate = DateTime.Now.AddMinutes(Constants.PasswordResetLifetimeMinutes)
            };
            context.PasswordResets.Add(passRst);
            context.SaveChanges();

            using (MailMessage mail = new MailMessage())
            {
                var credentials = SmtpCredentials.Get();
                mail.From = new MailAddress(credentials.Email);
                mail.To.Add(email);
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
        public IResult CreateUser(UserCreateDTO userDTO, [FromServices] ShopContext context)
        {

            if(userDTO.FirstName.Length < 1 || userDTO.LastName.Length < 1)
            {
                return Results.BadRequest("Invalid name");
            }
            if (!Validator.IsValidPassword(userDTO.Password)){
                return Results.BadRequest("Invalid password");
            }

            if(!Validator.isValidEmail(userDTO.Email)) {
                return Results.BadRequest("Invalid email");
            }

            if(context.Users.Any(u => u.Email.Equals(userDTO.Email)))
            {
                return Results.BadRequest("This email is already in use");
                //return Results.Problem("User with provided email address already exists: " + userDTO.Email);
            }

        var rng = new Random();
        var salt = rng.Next(int.MinValue, int.MaxValue).ToString();
        var hash = PasswordUtility.GenerateHash(userDTO.Password, salt);
        //var user = new User(0,userDTO.FirstName,userDTO.LastName,userDTO.Email,hash,salt);
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

        return Results.Ok("Created new user");
            
        }
        private string ProvidedDataIncorrect = "Provided data is incorrect";

        public IResult LoginUser(UserLoginDTO userDTO, [FromServices]ShopContext context, [FromServices]ITokenService tokenService)
        {
            var user = context.Users.SingleOrDefault(u => u.Email.Equals(userDTO.Email));
            if (user == null) return Results.BadRequest(ProvidedDataIncorrect);
            var hash = PasswordUtility.GenerateHash(userDTO.Password, user.PasswordSalt);
            if (!user.Password.Equals(hash)) return Results.BadRequest(ProvidedDataIncorrect);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };
            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Constants.RefreshTokenExpirationTimeDays);
            context.SaveChanges();

            return Results.Ok(new AuthenticatedResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }

    }
}
