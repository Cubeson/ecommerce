using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

public class PasswordReset
{
    [Key]
    public int Id { get; set; }
    public virtual User User { get; set; }
    public int UserId { get; set; }
    public string? ResetID { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
