using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medius.Model
{
    public class FAQ : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser Appl { get; set; }
        public string CreatedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public ApplicationUser User { get; set; }
        public string ModifiedBy { get; set; }
    }
}
