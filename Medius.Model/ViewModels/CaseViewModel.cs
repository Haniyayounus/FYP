using Medius.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medius.Model.ViewModels
{
    public class CaseViewModel
    {
        public virtual string Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public CaseType Type { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public string Application { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public string ImagePath { get; set; }
        public string DocumentPath { get; set; }
        [Required]
        public ModeofRegistration ModeofRegistration { get; set; }

        [NotMapped]
        public IFormFile Document { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }

        public int CityId { get; set; }

        [ForeignKey("ClaimId")]
        public int ClaimId { get; set; }

        [ForeignKey("IpFilterId")]
        public int IpFilterId { get; set; }
        public string UserId { get; set; }
        public string ModifiedBy { get; set; }

    }
    public class ChangeStatusViewModel
    {
        public string userId { get; set; }
        public string loggedInUserId { get; set; }
        public int caseId { get; set; }
        public Status Status { get; set; }
    }
    public class Document
    {
        public IFormFile FileDocument { get; set; }
        public string DocumentPath { get; set; }
    }

    public class Images
    {
        public IFormFile Image { get; set; }
        public string ImagePath { get; set; }
    }

    public class StripViewModel
    {
        public int caseId { get; set; }
        public string userId { get; set; }
        public ModeofRegistration mode { get; set; }
        public string stripeEmail { get; set; }
        public string stripeToken { get; set; }
        public int amount { get; set; }
        public string CardNumber { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
        public string Cvc { get; set; }
        
    }

public class PaymentDetailViewModel
    {
        public CaseType caseType { get; set; }
        public Status caseStatus { get; set; }
        public string userName { get; set; }
        public long amount { get; set; }
        public string email { get; set; }
        public string caseName { get; set; }
    }
}
