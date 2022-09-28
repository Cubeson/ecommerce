namespace ServerTests;
 public class UnitTest1
 {
     /*
      * Po stworzeniu u¿ytkownika, powinien o siê znaleœæ w bazie danych
      * 
      */
     [Fact]
     public async Task HelloWorld()
     {
         string testName = "HelloWorld";
         var app = TestWebApplicationFactory.Create(testName);
         var client = app.CreateClient();
         var response = await client.GetAsync("/Hello");
         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
     }

     [Fact]
     public async Task AddNewUserAsync()
     {
         string testName = "AddNewUserAsync";
         var app = TestWebApplicationFactory.Create(testName);
         //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
         var client = app.CreateClient();
         var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
         var result = await client.PostAsJsonAsync("/User/Create/", user1);
         var statusCode = result.StatusCode;
         var statusExpected = HttpStatusCode.OK;
         Assert.Equal(statusExpected, statusCode);

     }

     /*
      * Po stworzeniu u¿ytkownika, powinno byæ mo¿liwe zalogowanie siê tym samym has³em
      * 
      * After creating new user, it should be possible to log in with the same password
      */
     [Fact]
     public async Task CreateAndLoginUserAsync()
     {
         string testName = "CreateAndLoginUserAsync";
         var app = TestWebApplicationFactory.Create(testName);
         //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
         var client = app.CreateClient();
         var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
         var response1 = await client.PostAsJsonAsync("/User/Create", user1);
         var response2 = await client.PostAsJsonAsync("/User/Login" , new UserLoginDTO(user1.Email, user1.Password));
         var statusActual = response2.StatusCode;
         var statusExpected = HttpStatusCode.OK;
         Assert.Equal(statusExpected, statusActual);
     }
     /*
      * Inne has³a przy tworzeniu u¿ytkownika i logowaniu siê
      * 
      * Different passwords for creating user and logging in
      */
     [Fact]
     public async void CreateAndLoginUserBadPassword()
     {
         string testName = "CreateAndLoginUserBadPassword";
         var app = TestWebApplicationFactory.Create(testName);
         //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
         var client = app.CreateClient();
         var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass123");
         var response1 = await client.PostAsJsonAsync("/User/Create", user1);
         var response2 = await client.PostAsJsonAsync("/User/Login", new UserLoginDTO(user1.Email, "BadPassword"));
         var statusActual = response2.StatusCode;
         var statusExpected = HttpStatusCode.BadRequest;
         Assert.Equal(statusExpected, statusActual);
     }
     /*
      * Utworzenie u¿ytkownika z emailem istniej¹cym ju¿ w bazie danych nie powinno byæ mo¿liwe
      * 
      * Creating an user with an email that already exists in database shouldn't be possible
      */
     [Fact]
     public async void CreateUserWithExistingEmail()
     {
         string testName = "CreateUserWithExistingEmail";
         var app = TestWebApplicationFactory.Create(testName);
         //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
         var client = app.CreateClient();

         var email = "JanKowalski@gmail.com";
         var user1 = new UserCreateDTO("Jan", "Kowalski", email, "pass123");
         var user2 = new UserCreateDTO("Anna", "Kowalska", email, "coolpass1");
         var response1 = await client.PostAsJsonAsync("/User/Create",user1);
         var response2 = await client.PostAsJsonAsync("/User/Create", user2);
         var statusActual = response2.StatusCode;
         var statusExpected = HttpStatusCode.BadRequest;
         Assert.Equal(statusExpected, statusActual);

     }
 }
