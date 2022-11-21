using System.Data;

namespace Server.Utility;
public static class TokenUtility
{
    public static string? GetAuthTokenFromRequest(HttpRequest httpRequest)
    {
        return httpRequest
            .Headers
            .Authorization[0]?
            .Substring("Bearer ".Length);
    }
}
