using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Model
{
    public class Notification : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
