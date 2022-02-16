using Checked.Data;
using Checked.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Checked.Servicos.InviteService
{
    public class InviteService
    {
        private readonly CheckedDbContext _context;
        public InviteService(CheckedDbContext context)
        {
            _context = context;
        }
        public async Task<string> Invite(Invite invite)
        {
            string message = string.Empty;
            try
            {
                var result = await _context.Invites.Where(x => x.Email.Equals(invite.Email)).FirstOrDefaultAsync();
                if(result == null)
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

        public async Task<List<ApplicationUser>> GetUsersAsync(string organization)
        {
           return await _context.Users.Where(c => c.OrganizationId == organization).ToListAsync();
        }

    }
}
