using Server.Data;
using Server.DTO;
using System.Security.Cryptography;
using Server.Models;
using Server.Utility;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Server.Services.TokenService;
using System.Security.Claims;

namespace Server.Api
{
    public sealed class UserApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapPost("api/User/Create/", CreateUser);
            app.MapPost("api/User/Login/", LoginUser);
        }
        public IResult CreateUser(UserCreateDTO userDTO, ShopContext context)
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
            using(var sha256 = SHA256.Create())
            {
                var rng = new Random();
                var salt = rng.Next(int.MinValue, int.MaxValue).ToString();
                var hash = PasswordUtility.GenerateHash(userDTO.Password, salt);
                var user = new User(0,userDTO.FirstName,userDTO.LastName,userDTO.Email,hash,salt);
                context.Users.Add(user);
                context.SaveChanges();
                return Results.Ok("Created new user");
            }
            
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
