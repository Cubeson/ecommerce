using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class Cart
{
    [Key]
    public int Id { get; set; }
    public required virtual User User { get; set; }
    public virtual ICollection<CartItem> CartItems { get;set; } = new HashSet<CartItem>();
}
