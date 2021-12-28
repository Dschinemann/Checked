
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

    }
}