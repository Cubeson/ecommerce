using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;
public class OrderItem
{
    [Key] public int Id { get; set; }
    public string ProductName { get; set; }
    [Required] public Product Product { get; set; }
    public int ProductId { get; set; }
    [Column(TypeName = "decimal(8,2)")]
    public decimal ItemPrice { get; set; }
    public virtual Order Order { get; set; }
    public int OrderId { get; set; }
    public int Quantity { get; set; } = 1;
}
