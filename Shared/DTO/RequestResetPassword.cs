namespace Shared.DTO;
public sealed class RequestResetPasswordUnity
{
    public string Email;
}
public sealed class RequestResetPassword
{
    public string? Email { get; set; } = string.Empty;
}
