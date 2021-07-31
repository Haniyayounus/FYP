using Medius.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Medius.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Claims> Claims { get; set; }
        public DbSet<IpFilter> IpFilters { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
    }
}
