namespace Shared.DTO
{
    //public class TokenModelDTOUnity
    //{
    //    public string AuthToken = string.Empty;
    //    public string RefreshToken = string.Empty;
    //}

    public class TokenModelDTO
    {
        public string? AuthToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}