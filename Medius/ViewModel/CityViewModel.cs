using System.ComponentModel.DataAnnotations;

namespace Medius.ViewModel
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
}
