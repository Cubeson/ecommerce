namespace Shared.DTO;
public class AuthenticatedResponseUnity
{
    public string Token;
    public string RefreshToken;
}
public class TokenApiModelUnity
{
    public string AccessToken;
    public string RefreshToken;
}

public class AuthenticatedResponse
{
    public string? Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
}
public class TokenApiModel
{
    public string? AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
}
