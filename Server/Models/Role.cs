using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<User> Users { get; set; }
    }
}
