using Server.Data;
using Server.DTO;
using System.Security.Cryptography;
using Server.Models;
using Server.Utility;

namespace Server.Endpoints
{
    public static class UserEndpoint
    {
        internal static void Register(WebApplication app)
        {
            app.MapPost("/User/Create/", CreateUser);
            app.MapPost("/User/Login/", LoginUser);
        }
        public static IResult CreateUser(UserCreateDTO userDTO, ShopContext context)
        {
            if(context.Users.Any(u => u.Email.Equals(userDTO.Email)))
            {
                return Results.Ok(new ResponseBody("1", "This email is already in use"));
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
                return Results.Ok(new ResponseBody("0", "Created new user"));
            }
            
        }
        private static string ProvidedDataIncorrect = "Provided data is incorrect";

        public static IResult LoginUser(UserLoginDTO userDTO, ShopContext context)
        {
            User? userDB = null;
            try
            {
                userDB = context.Users.First(u => u.Email.Equals(userDTO.Email));
            }
            catch(InvalidOperationException)
            {}
            //if(userDB == null) return Results.Problem(ProvidedDataIncorrect);
            if (userDB == null) return Results.Ok(new ResponseBody("1", ProvidedDataIncorrect));
            var hash = PasswordUtility.GenerateHash(userDTO.Password, userDB.PasswordSalt);
            if (!userDB.Password.Equals(hash)) return Results.Ok(new ResponseBody("1", ProvidedDataIncorrect));
            return Results.Ok(new ResponseBody("0", "Logged in: " + userDB.FirstName + " " + userDB.LastName));
        }

    }
}
