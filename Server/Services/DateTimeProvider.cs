namespace Server.Services;
public class DateTimeProvider
{
    private readonly DateTime? _dateTime = null;
    public DateTimeProvider() { }
    public DateTimeProvider(DateTime dateTime)
    {
        _dateTime = dateTime;
    }
    public DateTime Now => _dateTime ?? DateTime.Now;
}
