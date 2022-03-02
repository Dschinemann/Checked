using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Checked.Servicos.InviteService
{
    public class InviteService
    {
        private readonly CheckedDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public InviteService(CheckedDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<string> Invite(Invite invite)
        {
            string message = string.Empty;
            try
            {
                var result = await _context.Invites.Where(x => x.Email.Equals(invite.Email)).FirstOrDefaultAsync();
                if (result == null)
                {
                    _context.Add(invite);
                    await _context.SaveChangesAsync();
                    message = "Convite enviado";
                    return message;
                }
                else
                {
                    message = "Um convite foi reenviado para este destinatário";
                    return message;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<List<UsersInRoleViewModel>> GetUsersAsync(string organization, string userId)
        {
            List<UsersInRoleViewModel> models = new List<UsersInRoleViewModel>();
            var usersInRoleViewModel = await _context.Users
                .Where(c => c.OrganizationId.Equals(organization) && !c.Id.Equals(userId))                
                .ToListAsync();
            foreach (var user in usersInRoleViewModel)
            {
                models.Add(new UsersInRoleViewModel()
                {
                    Email = user.Email,
                    Role = await _userManager.GetRolesAsync(user),
                    UserId = user.Id,
                    Username = user.Name
                });
            };
            return models;
        }
    }
}
