using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public string? AuthToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}
