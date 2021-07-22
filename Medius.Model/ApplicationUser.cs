using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Medius.Model
{
    public class ApplicationUser : IdentityUser
    {
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(Microsoft.AspNet.Identity.UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here
        //    userIdentity.AddClaim(new Claim("CityId", CityId.ToString()));
        //    return userIdentity;
        //}
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
        //    // Add custom user claims here
        //    userIdentity.AddClaim(new Claim("CityId", CityId.ToString()));
        //    return userIdentity;
        //}
        [Required]
        public string Name { get; set; } 
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Cnic { get; set; }
        public string Contact { get; set; }
        [NotMapped]
        public IFormFile ProfilePicture { get; set; }
        public string ImagePath { get; set; }
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        [NotMapped]
        public string Role { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Active { get; set; }
    }
}
