using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual User User { get; set; }
    public int UserId { get; set; }

    public string? BrokerOrderId { get; set; }
    public bool IsCompleted { get; set; } = false;

}
