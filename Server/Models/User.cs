using Server.Utility;
using System.ComponentModel.DataAnnotations;
using Shared.Validators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public bool IsEmailConfirmed = false;
        //public string PhoneNumber { get; set; } = string.Empty;
        public string Password {get; set; } = string.Empty;
        public string PasswordSalt {get; set; } = string.Empty;

        [InverseProperty("User")]
        public virtual ICollection<PasswordReset>? PasswordResets { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserSession>? UserSessions { get; set; }
        
        public bool SetPassword(string? password)
        {
            if (!Validators.IsValidPassword(password)) return false;
            var rng = new Random();
            var salt = rng.Next(int.MinValue, int.MaxValue).ToString();
            var hashed = StringHasher.HashString(password,salt);
            Password = hashed;
            PasswordSalt = salt;
            return true;
        }


    }
}
