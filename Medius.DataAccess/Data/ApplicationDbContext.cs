using Medius.Model;
using Medius.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<IpFilter> IpFilters { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
