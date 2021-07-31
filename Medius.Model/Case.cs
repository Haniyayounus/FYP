using Medius.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medius.Model
{
    public class Case : BaseEntity<int>
    {
            [Key]
            public override int Id { get; set; }
            
            [Required]
            public string Title { get; set; }
            public string Description { get; set; }
            public CaseType Type { get; set; }
            public string Contact { get; set; }
            public string Application { get; set; }
            public Status Status { get; set; }
            public string ImagePath { get; set; }
            public string DocumentPath { get; set; }
            public ModeofRegistration ModeofRegistration { get; set; }

            [NotMapped]
            public IFormFile FileDocument { get; set; }
            [NotMapped]
            public IFormFile ProfilePicture { get; set; }

        //relationships
            [ForeignKey("CityId")]
            public City City { get; set; }
            
            [ForeignKey("ClaimId")]
            public Claims Claim { get; set; }
            
            [ForeignKey("IpFilterId")]
            public IpFilter IpFilter { get; set; }
             public int IpFilterId { get; set; }

            [ForeignKey("UserId")]
            public ApplicationUser Appl { get; set; }
            public string UserId { get; set; }
            [ForeignKey("ModifiedBy")]
            public ApplicationUser Admin { get; set; }
            public string ModifiedBy { get; set; }


    }
}
