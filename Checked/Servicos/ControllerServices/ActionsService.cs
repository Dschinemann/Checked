using Checked.Data;
using Checked.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Checked.Servicos.ControllerServices
{
    public class ActionsService
    {
        private readonly CheckedDbContext _context;
        public ActionsService(CheckedDbContext context)
        {
            _context = context;
        }
        public async Task<List<Models.Models.Action>> ListActionAsync(int? id)
        {
            var result = await _context.Actions
                .Where(x => x.OccurrenceId == id)
                .ToListAsync();

            return result;
        }
    }
}
