namespace Shared.DTO;
public class CreateAccountResponseDTOUnity
{
    public int Error = 0;
    public string Message = string.Empty;
}

public class CreateAccountResponseDTO
{
    public int? Error { get; set; } = 0;
    public string? Message { get; set; } = string.Empty;
}

