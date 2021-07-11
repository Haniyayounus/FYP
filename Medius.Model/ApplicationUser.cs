using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cnic { get; set; }
        public string Contact { get; set; }
        [NotMapped]
        public IFormFile ProfilePicture { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
