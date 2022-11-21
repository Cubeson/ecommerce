namespace Server;

public class SmtpSettings
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class SmtpSingleton
{
    private SmtpSingleton() { }
    private static SmtpSettings? _settings;
    public static void Set(SmtpSettings settings) => _settings = settings;
    public static SmtpSettings Get()
    {
        if (_settings == null) throw new InvalidOperationException("Settings are not yet set");
        return _settings;
    }
}
