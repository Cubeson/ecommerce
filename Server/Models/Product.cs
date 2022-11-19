using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Server.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }


    }
}
