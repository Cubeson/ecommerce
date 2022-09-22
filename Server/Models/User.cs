using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public sealed record User
    {
        public User() { }
        public User(int id, string firstName, string lastName, string email, string password, string passwordSalt)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            PasswordSalt = passwordSalt;
        }

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public string PhoneNumber { get; set; } = string.Empty;
        public string Password {get; set; } = string.Empty;
        public string PasswordSalt {get; set; } = string.Empty;
        


    }
}
