using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; }


}