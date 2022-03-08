using Checked.Data;
using Checked.Models.Models;
using Checked.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace Checked.Servicos.ControllerServices
{
    public class TaskService
    {
        private readonly CheckedDbContext _context;
        public TaskService(CheckedDbContext context)
        {
            _context = context;
        }

        public async Task<List<Occurrence>> GetOccurrencesAsync(string organizationId, string userId)
        {
            var result = await _context.Occurrences
                .Where(c => c.OrganizationId.Equals(organizationId))
                .Where(c => c.AppraiserId.Equals(userId))
                .Include(o => o.Tp_Ocorrencia)
                .ToListAsync();
            return result;
        }

        public async Task<List<Plan>> GetPlansStatusOpenAsync(string organizationId, string userId)
        {
            var result = await _context.Plans
                .Include(o => o.Actions)
                .Where(c => c.organizationId.Equals(organizationId))
                .Where(c => c.AccountableId.Equals(userId))
                .Where(c => !c.Actions.Any(c => c.TP_StatusId.Equals((int)TP_StatusEnum.Encerrado) && c.TP_StatusId.Equals((int)TP_StatusEnum.Cancelado)))
                .ToListAsync();
            return result;
        }

        public async Task<List<Plan>> GetPlansStatusFinishAsync(string organizationId, string userId)
        {
            var result = await _context.Plans
                .Include(o => o.Actions)
                .Where(c => c.organizationId.Equals(organizationId))
                .Where(c => c.AccountableId.Equals(userId))
                .Where(c => c.Actions.Any(c => c.TP_StatusId.Equals((int)TP_StatusEnum.Encerrado) && c.TP_StatusId.Equals((int)TP_StatusEnum.Cancelado)))
                .ToListAsync();
            return result;
        }

        public async Task<List<Models.Models.Action>> GetActionsStatusOpenAsync(string organizationId, string userId)
        {
            var result = await _context.Actions
                .Where(c => c.OrganizationId.Equals(organizationId))
                .Where(c => c.WhoId.Equals(userId))
                .Where(c => !c.TP_StatusId.Equals((int)TP_StatusEnum.Encerrado) && !c.TP_StatusId.Equals((int)TP_StatusEnum.Cancelado))
                .ToListAsync();
            return result;
        }

        public async Task<List<Models.Models.Action>> GetActionsStatusFinishAsync(string organizationId, string userId)
        {
            var result = await _context.Actions
                .Where(c => c.OrganizationId.Equals(organizationId))
                .Where(c => c.WhoId.Equals(userId))
                .Where(c => c.TP_StatusId.Equals((int)TP_StatusEnum.Encerrado))
                .ToListAsync();
            return result;
        }
    }
}
