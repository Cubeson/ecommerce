using Server.Api;
using Server.Services.SmtpService;
using Microsoft.AspNetCore.Http.HttpResults;
using Server.Services.TokenService;
using Server.Services;
using Server.Utility;
namespace ServerTests;
public class UserApiTests : IClassFixture<DatabaseFixture>
{
    UserApi _userApi = new UserApi();
    static JWTSettings jwtSettings = new JWTSettings() { Audience = "test" ,Issuer="test",Key="TestSecretKey123456789123456789123456789"};
    ITokenService _tokenService = new TokenService(jwtSettings);
    ShopContext _shopContext;
    ISmtpService _smtpService = new SmtpTestService();
    public UserApiTests(DatabaseFixture dbFixture) {
        _shopContext = dbFixture.shopContext;
    }
    [Fact] public void CreatingNewUser_ShouldCreateAUser()
    {
        var user = new UserCreateDTO{Email="test1@gmail.com",FirstName="Test",LastName="Test",Password="Pass1234"};
        var res = (Ok<GenericResponseDTO>)_userApi.CreateUser(user, _shopContext, _smtpService);
        Assert.Equal(200,res.StatusCode);
        var userDB = _shopContext.Users.Single(u => u.Email == user.Email);
        Assert.Equal(user.Email, userDB.Email);
    }
    [Fact] public void CreatingNewUserWithBadEmail_ShouldNotCreateAUser()
    {
        var user = new UserCreateDTO { Email = "bad", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        var res = (BadRequest<GenericResponseDTO>)_userApi.CreateUser(user, _shopContext, _smtpService);
        Assert.Equal(400,res.StatusCode);
        var exists = _shopContext.Users.Any(u => u.Email==user.Email);
        Assert.False(exists);
    }
    [Fact] public void CreatingNewUserWithBadPassword_ShouldNotCreateAUser()
    {
        var user = new UserCreateDTO { Email = "test2@gmail.com", FirstName = "Test", LastName = "Test", Password = "1" };
        var res = (BadRequest<GenericResponseDTO>)_userApi.CreateUser(user, _shopContext, _smtpService);
        Assert.Equal(400, res.StatusCode);
        var exists = _shopContext.Users.Any(u => u.Email == user.Email);
        Assert.False(exists);
    }
    [Fact] public void CreatingNewUserWithExistingEmail_ShouldNotCreateAUser()
    {
        var user1 = new UserCreateDTO { Email = "test3@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        var user2 = new UserCreateDTO { Email = "test3@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };_userApi.CreateUser(user1, _shopContext, _smtpService);
        var res2 = (BadRequest<GenericResponseDTO>)_userApi.CreateUser(user2, _shopContext, _smtpService);
        Assert.Equal(400, res2.StatusCode);
        var count = _shopContext.Users.Count(u => u.Email == user2.Email);
        Assert.Equal(1,count);
    }
    [Fact] public void SendingCorrectLoginData_ShouldLogin()
    {
        var user = new UserCreateDTO { Email = "test4@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        _userApi.CreateUser(user, _shopContext, _smtpService);
        var resp2 = (Ok<TokenModelDTO>)_userApi.LoginUser(new UserLoginDTO { Email = user.Email, Password = user.Password }, _shopContext, _tokenService, new DateTimeProvider());
        Assert.Equal(200, resp2.StatusCode);
        Assert.NotNull(resp2.Value.AuthToken);
    }
    [Fact] public void SendingWrongLoginData_ShouldNotLogin()
    {
        var user = new UserCreateDTO { Email = "test5@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        _userApi.CreateUser(user, _shopContext, _smtpService);
        var resp2 = (BadRequest<string>)_userApi.LoginUser(new UserLoginDTO { Email = user.Email, Password="BadPassword" }, _shopContext, _tokenService, new DateTimeProvider());
        Assert.Equal(400, resp2.StatusCode);
    }
    [Fact] public void WhenUsingValidResetCode_ShouldChangePassword()
    {
        var user = new UserCreateDTO { Email = "test6@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        _userApi.CreateUser(user,_shopContext, _smtpService);
        var previousPassword = _shopContext.Users.Single(u => u.Email== user.Email).Password;
        _userApi.RequestResetPasswordCode(new RequestResetPasswordDTO { Email = user.Email },_shopContext,_smtpService, new DateTimeProvider());
        var resetCode = _shopContext.PasswordResets.Include(c => c.User).First(c => c.User.Email== user.Email).ResetID;
        string newPassword = "NewPassword123";
        var resp = (Ok<GenericResponseDTO>)_userApi.ResetPassword(new ResetPasswordCredentialsDTO { ResetId= resetCode,Password = newPassword },_shopContext, new DateTimeProvider());
        Assert.Equal(200,resp.StatusCode);
        var userDB = _shopContext.Users.Single(u => u.Email == user.Email);
        Assert.NotEqual(previousPassword, userDB.Password);
    }
    [Fact]
    public void WhenUsingWrongResetCode_ShouldNotChangePassword()
    {
        var user = new UserCreateDTO { Email = "test7@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        _userApi.CreateUser(user, _shopContext, _smtpService);
        _userApi.RequestResetPasswordCode(new RequestResetPasswordDTO { Email = user.Email }, _shopContext, _smtpService, new DateTimeProvider());
        var previousPassword = _shopContext.Users.Single(u => u.Email == user.Email).Password;
        var resetCode = "WrongResetCode";
        string newPassword = "NewPassword123";
        var resp = (BadRequest<GenericResponseDTO>)_userApi.ResetPassword(new ResetPasswordCredentialsDTO { ResetId = resetCode, Password = newPassword }, _shopContext, new DateTimeProvider());
        Assert.Equal(400, resp.StatusCode);
        var userDB = _shopContext.Users.Single(u => u.Email == user.Email);
        Assert.Equal(previousPassword, userDB.Password);
    }

}
