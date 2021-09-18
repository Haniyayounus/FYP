using Medius.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Medius.Model.ViewModel
{
    public class CityViewModel
    {
       
        public virtual int Id { get; set; }
        [Required]
        public string Name { get; set; }
    } 
    public class UpdateCityViewModel : CityViewModel
    {
        [Required]
        public override int Id { get; set; }
    }
    public class ClaimViewModel
    {

        public virtual int Id { get; set; }
        [Required]
        public string Description { get; set; }
    }
    public class UpdateClaimViewModel : ClaimViewModel
    {
        [Required]
        public override int Id { get; set; }
    }
    public class FAQViewModel
    {

        public virtual int Id { get; set; }
        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public string ModifiedBy { get; set; }
    }
    public class UpdateFAQViewModel : FAQViewModel
    {
        [Required]
        public override int Id { get; set; }
    }
    public class ArchiveFAQVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public string ModifiedBy { get; set; }
    }
    public class IPFilterViewModel
    {

        public virtual int Id { get; set; }
        [Required]
        public FilterType Type { get; set; }

        [Required]
        public string Name { get; set; }
    }
    public class UpdateIPFilterViewModel : IPFilterViewModel
    {
        [Required]
        public override int Id { get; set; }
    }
    public class NotificationViewModel
    {

        [Required]
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description{ get; set; }
    }
}
