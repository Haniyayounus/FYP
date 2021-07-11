using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medius.Model
{
    public class City : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }

        [Display(Name = "City Name")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
