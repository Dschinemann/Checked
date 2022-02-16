using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Checked.Models.Models;
using Checked.Models.Types;
using Checked.Models.ViewModels;

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
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Invite> Invites { get; set; }

        public DbSet<TP_Ocorrencia> TP_Ocorrencias { get; set; }
        public DbSet<TP_Status> TP_Status { get; set; }
        public DbSet<TP_StatusOccurence> TP_StatusOccurences { get; set; }

        public DbSet<State> States { get; set; } 
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; } 

    }
}