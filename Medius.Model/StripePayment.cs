using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medius.Model
{
    public class StripePayment : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        public int Amount { get; set; }
        public string Token { get; set; }
        [ForeignKey("CaseId")]
        public Case Case { get; set; }
        public int CaseId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }
    }
}
