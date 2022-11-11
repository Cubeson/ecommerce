namespace Shared.DTO;
public class CreateAccountResponseUnity
{
    public int Error = 0;
    public string Message = string.Empty;
}

public class CreateAccountResponse
{
    public int? Error { get; set; } = 0;
    public string? Message { get; set; } = string.Empty;
}

