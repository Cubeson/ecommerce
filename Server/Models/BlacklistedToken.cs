using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class BlacklistedToken
    {
        [Key]
        public int Id { get; set; }
        public required string AuthToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
