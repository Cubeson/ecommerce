namespace Shared.DTO;
//public class AuthenticatedResponseUnity
//{
//    public string Token;
//    public string RefreshToken;
//}
public class TokenModelUnity
{
    public string AuthToken;
    public string RefreshToken;
}

//public class AuthenticatedResponse
//{
//    public string? Token { get; set; } = string.Empty;
//    public string? RefreshToken { get; set; } = string.Empty;
//}
public class TokenModel
{
    public string? AuthToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
}
