
using Microsoft.EntityFrameworkCore;
using Checked.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Checked.Data
{
    public class CheckedDbContext : IdentityDbContext//DbContext
    {
        public CheckedDbContext(DbContextOptions<CheckedDbContext> options) : base(options)
        {

        }
        DbSet<User> Users { get; set; }

    }
}