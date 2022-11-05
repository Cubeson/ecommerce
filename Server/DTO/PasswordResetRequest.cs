namespace Server.DTO
{
    public class PasswordResetRequest
    {
        public string? ResetId { get; set; }
        public string? Password { get; set; }
    }
}
