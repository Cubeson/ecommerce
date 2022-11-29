namespace Shared.DTO
{
    //public class UserCreateDTOUnity
    //{
    //    public string FirstName = string.Empty;
    //    public string LastName = string.Empty;
    //    public string Email = string.Empty;
    //    public string Password = string.Empty;
    //}
    public class UserCreateDTO
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}