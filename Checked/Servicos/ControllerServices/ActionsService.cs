using Checked.Data;
using Checked.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Action = Checked.Models.Models.Action;

namespace Checked.Servicos.ControllerServices
{
    public class ActionsService
    {
        private readonly CheckedDbContext _context;
        public ActionsService(CheckedDbContext context)
        {
            _context = context;
        }

        public async Task<List<Action>> GetAllAsync(string organizationId)
        {
            var actions = await _context.Actions
                .Include(o => o.TP_Status)
                .Where(c => c.OrganizationId.Equals(organizationId))
                .ToListAsync();
            return actions;
        }

        public IEnumerable<StatusQuantidade> CountPerStatus(List<Models.Models.Action> actions)
        {
            var result = actions
                    .GroupBy(c => c.TP_Status)
                    .Select(c => new StatusQuantidade { Status = c.Key.Name, Quantidade = c.Count() });
            return result;
        }
    }
}
