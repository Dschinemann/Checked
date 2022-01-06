
using Microsoft.EntityFrameworkCore;
using Checked.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Checked.Data
{
    public class CheckedDbContext : IdentityDbContext<ApplicationUser>//DbContext
    {
        public CheckedDbContext(DbContextOptions<CheckedDbContext> options) : base(options)
        {

        }
        public DbSet<Organization> Organizations{ get; set; }
        public DbSet<Occurrence> Occurrences { get; set;}
        public DbSet<Models.Models.Action> Actions { get; set; }

        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Occurrences)
        }*/

    }
}