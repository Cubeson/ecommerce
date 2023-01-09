namespace Shared.DTO
{
    public class TokenModelDTO
    {
        public string? AuthToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public int ExpiresInSeconds { get; set; }
    }
}