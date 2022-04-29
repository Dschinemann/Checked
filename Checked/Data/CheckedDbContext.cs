using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Checked.Models.Models;
using Checked.Models.Types;
using Checked.Models.ViewModels;
using Checked.Models.Statics;

namespace Checked.Data
{
    public class CheckedDbContext : IdentityDbContext<ApplicationUser>//DbContext
    {
        public CheckedDbContext(DbContextOptions<CheckedDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Organization>()
                .HasMany(b => b.Users);                                

            builder.Entity<Organization>()
                .HasOne(o => o.CreatedBy)
                .WithMany();

            builder.Entity<ApplicationUser>()
                .HasMany(o => o.Occurrences)
                .WithOne(o => o.ApplicationUser);

            builder.Entity<Occurrence>()
                .HasOne(o => o.Plan);
             
        }   

        public DbSet<Organization> Organizations{ get; set; }
        public DbSet<Occurrence> Occurrences { get; set;}
        public DbSet<Models.Models.Action> Actions { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Invite> Invites { get; set; }

        public DbSet<TP_Ocorrencia> TP_Ocorrencias { get; set; }
        public DbSet<TP_Status> TP_Status { get; set; }
        public DbSet<TP_StatusOccurence> TP_StatusOccurences { get; set; }

        public DbSet<State> States { get; set; } 
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        
        public DbSet<HelpDesk> HelpDesks { get; set; }

        public DbSet<TP_TaskStatus> TP_TaskStatus { get; set; }
        public DbSet<Models.Models.Task> Tasks { get; set; }

    }
}