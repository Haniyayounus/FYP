using System.ComponentModel.DataAnnotations;

namespace Medius.Model
{
    public class Claims : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
    }
}
