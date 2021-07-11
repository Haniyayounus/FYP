using Medius.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Model
{
    public class IpFilter : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        public FilterType Type { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
