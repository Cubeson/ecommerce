namespace Server.DTO
{
    public sealed class UserCreateDTO
    {
        public UserCreateDTO() { }

        public UserCreateDTO(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
       // public string PasswordRepeat { get; set; } = string.Empty;

    }
}
