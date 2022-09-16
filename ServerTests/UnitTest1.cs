using Microsoft.AspNetCore.Http;
using Server.Utility;

namespace ServerTests
{
    public class UnitTest1
    {
        //private ShopContext _context;
        //public UnitTest1()
        //{
        //    _context = new ShopContext(ShopContext.GetInMemoryOptions("Main"));
        //}

        /*
         * Po stworzeniu u¿ytkownika, powinien o siê znaleœæ w bazie danych
         * 
         */
        [Fact]
        public void AddNewUser()
        {
            using (var context = new ShopContext(ShopContext.GetInMemoryOptions("AddNewUser")))
            {
                var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
                var result = UserEndpoint.CreateUser(user1, context);
                var responseBody = result.GetOkObjectResultValue<ResponseBody>();
                var statusActual = responseBody?.Status;
                var statusExpected = "0";
                Assert.Equal(statusExpected, statusActual);
                var found = context.Users.Any(u => u.Email.Equals(user1.Email));
                Assert.True(found);
            }

        }
        /*
         * Po stworzeniu u¿ytkownika, powinno byæ mo¿liwe zalogowanie siê tym samym has³em
         * 
         * After creating new user, it should be possible to log in with the same password
         */
        [Fact]
        public void CreateAndLoginUser()
        {
            using (var context = new ShopContext(ShopContext.GetInMemoryOptions("CreateAndLoginUser")))
            {
                var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
                UserEndpoint.CreateUser(user1, context);
                var result = UserEndpoint.LoginUser(new UserLoginDTO(user1.Email, user1.Password), context);
                var responseBody = result.GetOkObjectResultValue<ResponseBody>();
                var statusActual = responseBody?.Status;
                var statusExpected = "0";
                Assert.Equal(statusExpected, statusActual);
            }
        }
        /*
         * Inne has³a przy tworzeniu u¿ytkownika i logowaniu siê
         * 
         * Different passwords for creating user and logging in
         */
        [Fact]
        public void CreateAndLoginUserBadPassword()
        {
            using (var context = new ShopContext(ShopContext.GetInMemoryOptions("CreateAndLoginUserBadPassword")))
            {
                var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
                UserEndpoint.CreateUser(user1, context);
                var result = UserEndpoint.LoginUser(new UserLoginDTO(user1.Email, "BadPassword"), context);
                var responseBody = result.GetOkObjectResultValue<ResponseBody>();
                var statusActual = responseBody?.Status;
                var statusExpected = "1";
                Assert.Equal(statusExpected, statusActual);
            }
        }
        /*
         * Utworzenie u¿ytkownika z emailem istniej¹cym ju¿ w bazie danych nie powinno byæ mo¿liwe
         * 
         * Creating an user with an email that already exists in database shouldn't be possible
         */
        [Fact]
        public void CreateUserWithExistingEmail()
        {
            using (var context = new ShopContext(ShopContext.GetInMemoryOptions("CreateUserWithExistingEmail")))
            {
                var count = context.Users.Count();
                Assert.Equal(0, count);

                var email = "JanKowalski@gmail.com";
                var user1 = new UserCreateDTO("Jan", "Kowalski", email, "pass123");
                var user2 = new UserCreateDTO("Anna", "Kowalska", email, "coolpass1");
                var result1 = UserEndpoint.CreateUser(user1, context);
                var result2 = UserEndpoint.CreateUser(user2, context);
                var responseBody = result2.GetOkObjectResultValue<ResponseBody>();
                var statusActual = responseBody?.Status;
                var statusExpected = "1";
                Assert.Equal(statusExpected, statusActual);

            }
        }
    }
}