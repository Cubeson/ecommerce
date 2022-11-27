namespace Shared.DTO
{
    public class GenericResponseDTOUnity
    {
        public int Error = 0;
        public string Message = string.Empty;
    }

    public class GenericResponseDTO
    {
        public int? Error { get; set; } = 0;
        public string? Message { get; set; } = string.Empty;
    }
}