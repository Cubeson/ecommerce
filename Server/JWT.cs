using Microsoft.AspNetCore.Mvc;

namespace Server;

public class JWTSettings
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }

}
public class JWTSingleton
{
    private JWTSingleton() { }
    private static JWTSettings? _settings;
    public static void Set(JWTSettings settings) => _settings = settings;

    public static JWTSettings Get()
    {
        if (_settings == null) throw new InvalidOperationException("Settings are not yet set");
        return _settings;
    }
}
