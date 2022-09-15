using Microsoft.AspNetCore.Http;
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
         * Po stworzeniu u�ytkownika, powinien o si� znale�� w bazie danych
         * 
         */
        [Fact]
        public void AddNewUser()
        {
            using (var context = new ShopContext(ShopContext.GetInMemoryOptions("AddNewUser")))
            {
                var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
                var result = UserEndpoint.CreateUser(user1, context);
                var resultStatusCode = result.GetOkObjectResultStatusCode();
                var expectedStatusCode = Results.Ok().GetOkObjectResultStatusCode();
                Assert.Equal(expectedStatusCode, resultStatusCode);
                var found = context.Users.Any(u => u.Email.Equals(user1.Email));
                Assert.True(found);
            }

        }
        /*
         * Po stworzeniu u�ytkownika, powinno by� mo�liwe zalogowanie si� tym samym has�em
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
                var resultStatusCode = result.GetOkObjectResultStatusCode();
                var expectedStatusCode = Results.Ok().GetOkObjectResultStatusCode();
                Assert.Equal(expectedStatusCode, resultStatusCode);
            }
        }
        /*
         * Inne has�a przy tworzeniu u�ytkownika i logowaniu si�
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
                Assert.NotEqual(Results.Ok().GetOkObjectResultStatusCode(), result.GetOkObjectResultStatusCode());
            }
        }
        /*
         * Utworzenie u�ytkownika z emailem istniej�cym ju� w bazie danych nie powinno by� mo�liwe
         * 
         * Creating an user with an email that already exists in database shouldn't be possible
         */
        [Fact]
        public void CreateUserWithExistingEmail()
        {
            using (var context = new ShopContext(ShopContext.GetInMemoryOptions("CreateUserWithExistingEmail")))
            {
                var email = "JanKowalski@gmail.com";
                var user1 = new UserCreateDTO("Jan", "Kowalski", email, "pass123");
                var user2 = new UserCreateDTO("Anna", "Kowalska", email, "coolpass1");
                var result1 = UserEndpoint.CreateUser(user1, context);
                Assert.Equal(Results.Ok().GetOkObjectResultStatusCode(),result1.GetOkObjectResultStatusCode());
                var result2 = UserEndpoint.CreateUser(user2, context);
                Assert.NotEqual(Results.Ok().GetOkObjectResultStatusCode(), result2.GetOkObjectResultStatusCode());
                var count = context.Users.Count(x => x.Email.Equals(email));
                Assert.Equal(1, count);
            }
        }
    }
}