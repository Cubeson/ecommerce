using Server.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Server.Models
{
    public sealed record Product
    {
        public Product() { }
        public Product(int id, string name, string description, decimal price, DateTime dateCreated, DateTime dateModified)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        //public Model3DInfo? Model { get; set; }
        //public string Filename { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string FileFormat { get; set; } = "fbx";

    }
}
