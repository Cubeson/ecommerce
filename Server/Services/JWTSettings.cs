using Microsoft.AspNetCore.Mvc;

namespace Server.Services;

public class JWTSettings
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }

}