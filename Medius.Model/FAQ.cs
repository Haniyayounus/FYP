using System;
using System.ComponentModel.DataAnnotations;

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
    }
}
