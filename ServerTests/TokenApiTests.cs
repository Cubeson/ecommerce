using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Server.Api;
using Server.Services;
using Server.Services.SmtpService;
using Server.Services.TokenService;
using Server.Utility;
namespace ServerTests;
[Collection("Sequential")]
public class TokenApiTests : IClassFixture<DatabaseFixture>
{
    ShopContext _shopContext;
    TokenApi _tokenApi = new TokenApi();
    UserApi _userApi = new UserApi();
    static JWTSettings jwtSettings = new JWTSettings() { Audience = "test", Issuer = "test", Key = "TestSecretKey123456789123456789123456789" };
    ITokenService _tokenService = new TokenService(jwtSettings);
    ISmtpService smtpService = new SmtpTestService();
    public TokenApiTests(DatabaseFixture databaseFixture) {
        _shopContext = databaseFixture.shopContext;
    }
    [Fact] public void RefreshingAuthorization_WithValidRefreshToken_ShouldRefreshSession()
    {
        var userDTO = new UserCreateDTO() { Email="TestTokenEmail1@gmail.com",FirstName="Test",LastName="Test",Password = "Pass1234"};
        _userApi.CreateUser(userDTO, _shopContext, smtpService);
        var resp1 = (Ok<TokenModelDTO>)_userApi.LoginUser(new UserLoginDTO { Email = userDTO.Email, Password = userDTO.Password },_shopContext,_tokenService ,new DateTimeProvider());
        var resp2 = (Ok<TokenModelDTO>) _tokenApi.Refresh(resp1.Value,_shopContext, _tokenService, new DateTimeProvider());
        Assert.Equal(200,resp2.StatusCode);
    
    }
    [Fact] public void RefreshingAuthorization_WithExpiredRefreshToken_ShouldNotRefreshSession()
    {
        var userDTO = new UserCreateDTO() { Email = "TestTokenEmail2@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        _userApi.CreateUser(userDTO, _shopContext, smtpService);
        var resp1 = (Ok<TokenModelDTO>)_userApi.LoginUser(new UserLoginDTO { Email = userDTO.Email, Password = userDTO.Password }, _shopContext, _tokenService, new DateTimeProvider());
        var resp2 = (BadRequest)_tokenApi.Refresh(resp1.Value, _shopContext, _tokenService, new DateTimeProvider(DateTime.Now.AddHours(Constants.REFRESH_TOKEN_EXPIRATION_TIME_HOURS + 1)));
        Assert.Equal(400, resp2.StatusCode);
    }
    [Fact] public void RevokingToken_ShouldRevokeToken()
    {
        var userDTO = new UserCreateDTO() { Email = "TestTokenEmail3@gmail.com", FirstName = "Test", LastName = "Test", Password = "Pass1234" };
        _userApi.CreateUser(userDTO, _shopContext, smtpService);
        var resp1 = (Ok<TokenModelDTO>)_userApi.LoginUser(new UserLoginDTO { Email = userDTO.Email, Password = userDTO.Password }, _shopContext, _tokenService, new DateTimeProvider());
        var sessionId = _shopContext.UserSessions.Include(s => s.User).First(s => s.User.Email == userDTO.Email).Id;
        string authToken = resp1!.Value!.AuthToken;
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Headers.Authorization).Returns(new Microsoft.Extensions.Primitives.StringValues($"Bearer {authToken}"));
        var resp2 = (Ok)_tokenApi.Logout(mockRequest.Object, _shopContext);
        Assert.Equal(200,resp2.StatusCode);
        var isRevoked = _shopContext.UserSessions.First(s => s.Id == sessionId).IsRevoked;
        Assert.True(isRevoked);
    }
}
