using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Checked.Servicos.ControllerServices
{
    public class PlansService
    {
        private readonly CheckedDbContext _context;
        public PlansService(CheckedDbContext context)
        {
            _context = context;
        }
        public async Task<List<Plan>> GetPlansAsync(string organizationId)
        {
            var plans = await _context.Plans
                .Include(o => o.Actions)
                .Include(o => o.Occurrence)
                .Include(o => o.Accountable)
                .Where(c => c.organizationId == organizationId)
                .ToListAsync();
            return plans;
        }
        public async Task<Plan> GetPlanById(string id)
        {
            return await _context.Plans
                .Include(o => o.Occurrence)                
                .FirstAsync(o => o.Id == id);
        }
        public async Task UpdatePlanAsync(Plan plan)
        {
            if (plan != null)
            {
                try
                {
                    _context.Plans.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public IEnumerable<StatusQuantidade> GetSumActionPerStatus(List<Models.Models.Action> actions)
        {
            var result = actions
                .GroupBy(c => c.TP_Status)
                .Select(c => new StatusQuantidade { Status = c.Key.Name, Quantidade = c.Count() });
            return result;
        }
    }
}
