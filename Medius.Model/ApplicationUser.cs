using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Medius.Model
{
    public class ApplicationUser : IdentityUser
    {
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    //note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    //var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationType.ApplicationCookie);
        //    ////add custom user claims here
        //    //userIdentity.AddClaim(new Claim("RoleId", Role.ToString()));
        //    //return userIdentity;
        //}
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
