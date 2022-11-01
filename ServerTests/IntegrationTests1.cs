using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Net.Http.Json;
using System.Net.Mail;
using Xunit;

namespace ServerTests;
 public class IntegrationTests1
 {

    /// <summary>
    /// <para>Po stworzeniu u¿ytkownika, powinien o siê znaleœæ w bazie danych</para>
    /// </summary>
    [Fact]
     public async void AddNewUserAsync()
     {
        string testName = "AddNewUserAsync";
        var app = TestWebApplicationFactory.Create(testName);
        //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
        var client = app.CreateClient();
        var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass1234");
        var result = await client.PostAsJsonAsync("api/User/Create/", user1);
        var statusCode = result.StatusCode;
        var statusExpected = HttpStatusCode.OK;
        Assert.Equal(statusExpected, statusCode);

     }
    /// <summary>
    /// <para>Po stworzeniu u¿ytkownika, powinno byæ mo¿liwe zalogowanie siê tym samym has³em</para>
    /// <para>After creating new user, it should be possible to log in with the same password</para>
    /// </summary>
    [Fact]
     public async void CreateAndLoginUserAsync()
     {
        string testName = "CreateAndLoginUserAsync";
        var app = TestWebApplicationFactory.Create(testName);
        
        //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
        var client = app.CreateClient();
        var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass1234");
        var response1 = await client.PostAsJsonAsync("api/User/Create", user1);
        var response2 = await client.PostAsJsonAsync("api/User/Login", new UserLoginDTO(user1.Email, user1.Password));
        var statusActual = response2.StatusCode;
        var statusExpected = HttpStatusCode.OK;
        Assert.Equal(statusExpected, statusActual);
     }

    /// <summary>
    /// <para>Inne has³a przy tworzeniu u¿ytkownika i logowaniu siê</para>
    /// <para>Different passwords for creating user and logging in</para>
    /// </summary>
    [Fact]
     public async void CreateAndLoginUserBadPassword()
     {
        string testName = "CreateAndLoginUserBadPassword";
        var app = TestWebApplicationFactory.Create(testName);
        //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
        var client = app.CreateClient();
        var user1 = new UserCreateDTO("Jan", "Kowalski", "JanKowalski@gmail.com", "pass1234");
        var response1 = await client.PostAsJsonAsync("api/User/Create", user1);
        var response2 = await client.PostAsJsonAsync("api/User/Login", new UserLoginDTO(user1.Email, "BadPassword"));
        var statusActual = response2.StatusCode;
        var statusExpected = HttpStatusCode.BadRequest;
        Assert.Equal(statusExpected, statusActual);
     }

    /// <summary>
    /// <para>Utworzenie u¿ytkownika z emailem istniej¹cym ju¿ w bazie danych nie powinno byæ mo¿liwe</para> 
    /// <para>Creating an user with an email that already exists in database shouldn't be possible</para>
    /// </summary>
    [Fact]
     public async void CreateUserWithExistingEmail()
     {
        string testName = "CreateUserWithExistingEmail";
        var app = TestWebApplicationFactory.Create(testName);
        //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
        var client = app.CreateClient();

        var email = "JanKowalski@gmail.com";
        var user1 = new UserCreateDTO("Jan", "Kowalski", email, "pass1234");
        var user2 = new UserCreateDTO("Anna", "Kowalska", email, "coolpass1");
        var response1 = await client.PostAsJsonAsync("api/User/Create",user1);
        var response2 = await client.PostAsJsonAsync("api/User/Create", user2);
        var statusActual = response2.StatusCode;
        var statusExpected = HttpStatusCode.BadRequest;
        Assert.Equal(statusExpected, statusActual);

     }
    /// <summary>
    /// <para>Stworzenie nowego produktu wysy³aj¹c wszystkie informacje poprawnie powinno zakoñczyæ siê sukcesem</para>
    /// <para>Creating a new product by sending all information correctly should end with success </para>
    /// </summary>
    [Fact]
    public async void AddNewProduct()
    {
        string testName = "AddNewProduct";
        var app = TestWebApplicationFactory.Create(testName);
        //var context = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase(testName).Options);
        var client = app.CreateClient();
        MultipartFormDataContent form = new MultipartFormDataContent
        {
            { new StringContent("Name"), "name" },
            { new StringContent("Desc"), "description" },
            { new StringContent("10"), "price" }
        };

        var fThumbnail = File.ReadAllBytes("../../../Resources/shiba/thumbnail.png");
        form.Add(new ByteArrayContent(fThumbnail, 0, fThumbnail.Length), "fileThumbnail","thumbnail.png");

        var fModel = File.ReadAllBytes("../../../Resources/shiba/model.fbx");
        form.Add(new ByteArrayContent(fModel, 0, fModel.Length), "fileModel", "model.fbx");

        var fArchive = File.ReadAllBytes("../../../Resources/shiba/archive.zip");
        form.Add(new ByteArrayContent(fArchive, 0, fArchive.Length), "fileTexturesArchive", "archive.zip");
        var response = await client.PostAsync("api/Product/add", form);
        var msg = await response.Content.ReadAsStringAsync();
        var statusActual = response.StatusCode;
        var statusExpected = HttpStatusCode.OK;
        Assert.Equal(statusExpected, statusActual);
    }
    /// <summary>
    /// <para>Stworzenie u¿ytkownika z nieprawid³owym adresem email powinno byæ niemo¿liwe</para>
    /// <para>Creating an user with an invalid email address should not be possible</para>
    /// </summary>
    [Fact]
    public async void RegisterUserInvalidEmail()
    {
        string testName = "RegisterUserInvalidEmail";
        var app = TestWebApplicationFactory.Create(testName);
        var client = app.CreateClient();
        // In this case a false positive is preferable to false negative
        string[] invalidEmails = { "invalid", "fnfno@","ifnw@@@","nice@.dadafwfwg","@", "#@%^%#$@#$@#.com", "email@example.com (Joe Smith)", "“(),:;<>[\\]@example.com" };

        var statusExpected = HttpStatusCode.BadRequest;
        foreach (string invalidEmail in invalidEmails)
        {
            UserCreateDTO user = new UserCreateDTO("FirstName","LastName",invalidEmail,"Pass1234");
            var response = await client.PostAsJsonAsync("api/User/Create", user);
            var statusActual = response.StatusCode;
            
            Assert.Equal(statusExpected, statusActual);
        }
    }
 }
