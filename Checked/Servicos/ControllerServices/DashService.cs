using Checked.Data;
using Checked.Models.Enums;
using Checked.Models.Models;
using Checked.Models.Types;
using Checked.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Checked.Servicos
{
    public class DashService
    {
        private readonly CheckedDbContext _context;

        public object ActionResume { get; private set; }

        public DashService(CheckedDbContext context)
        {
            _context = context;
        }
        public async Task<OccurrenceResume> GetOccurrenceAsync(string id)
        {
            var resume = await _context.Occurrences
                .Where(c => c.OrganizationId == id)
                .GroupBy(d => d.Id)
                .Select(e => new
                {
                    Count = e.Key,
                    TotalCost = e.Sum(s => s.Cost)
                })
                .ToListAsync();


            OccurrenceResume ocResume = new OccurrenceResume()
            {
                Count = resume.Count,
                TotalCost = resume.Sum(c => c.TotalCost)
            };
            return ocResume;
        }
        public async Task<Organization> GetOrganizationAsync(string id)
        {
            Organization org = await _context.Organizations
                .Where(c => c.Id == id)
                .FirstAsync();
            return org;
        }
        public async Task<List<ActionsSummary>> GetCountActionsAsync(string id)
        {
            var ids = await _context.Plans
              .Where(c => c.organizationId == id)
              .Select(c => c.Id)
              .ToArrayAsync();

            var actions = await _context.Actions
                .Where(c => ids.Contains(c.PlanId))
                .Include(o => o.TP_Status)
                .GroupBy(c => c.TP_Status.Id)
                .Select(c => new { status = c.Key, total = c.Count() })
                .ToListAsync();

            List<ActionsSummary> list = new List<ActionsSummary>();

            foreach (var action in actions)
            {
                list.Add(new ActionsSummary
                {
                    Quantidade = action.total,
                    Tipo = Enum.GetName(typeof(TP_StatusEnum), action.status)
                });
            }
            return list;
        }

        public async Task<PlansSummary> GetCountPlansAsync(string id)
        {
            var plansEncerradas = await _context.Plans
                .Where(c => c.organizationId == id && !_context.Actions.Any(b => b.TP_StatusId != ((int)TP_StatusEnum.Encerrado) && b.PlanId == c.Id))
                .CountAsync(c => c.organizationId == id);

            var countPlans = await _context.Plans
                .Where(c => c.organizationId == id)
                .CountAsync();

            var idPlans = await _context.Plans
              .Where(c => c.organizationId == id)
              .Select(c => c.Id)
              .ToArrayAsync();

            var custoPlans = await _context.Actions
                .Where(c => idPlans.Contains(c.PlanId))
                .SumAsync(c => c.HowMuch);

            PlansSummary plansResume = new PlansSummary()
            {
                CustoTotal = custoPlans,
                PlanCriados = countPlans,
                QuantEncerrados = (int)plansEncerradas
            };
            return plansResume;
        }

        public async Task<DeadLineActions> GetDeadlineAsync(string id)
        {
            var idPlans = await _context.Plans
                .Where(c => c.organizationId == id)
                .Select(c => c.Id)
                .ToArrayAsync();
            var result = await _context.Actions
                .Where(c => idPlans.Contains(c.PlanId))
                .ToListAsync();

            DeadLineActions deadLineActions = new DeadLineActions();

            foreach(var item in result)
            {
                if(deadLineActions.DeadLine.CompareTo(new DateTime()) == 0)
                {
                    deadLineActions.DeadLine = item.NewFinish;
                    deadLineActions.Name = item.What;
                }
                else
                {
                    if(deadLineActions.DeadLine.CompareTo(item.NewFinish) < 0 && item.TP_StatusId != 3)
                    {
                        deadLineActions.DeadLine = item.NewFinish;
                        deadLineActions.Name = item.What;
                    }
                }
            }
            return deadLineActions;
        }
        
        public async Task<List<SummaryPerWeek>> GetCostPerMonthAsync(string id)
        {
            return await _context.Occurrences
                .Where(c => c.OrganizationId.Equals(id))
                .GroupBy(c => new { weekNumber = (c.CreatedAt.Day)/7 })
                .Select(c => new SummaryPerWeek { Week = $"Semana {c.Key.weekNumber}", Cost = c.Sum(x => x.Cost)})
                .ToListAsync();
        }
        
        public async Task<List<SummaryOccurrencesPerStatus>> GetOccurrencesPerStatus(string id)
        {
           return await _context.Occurrences
                .Where(c => c.OrganizationId.Equals(id))
                .Include(o => o.Status)
                .GroupBy(o => new {o.StatusId, o.Status.Name})
                .Select(x => new SummaryOccurrencesPerStatus { Status = x.Key.Name, Quantidade = x.Count()})
                .ToListAsync();                
        }
        
        public async Task<List<SummarryOccurrencePerName>> GetCostPerTypeOccurrences(string id)
        {
           return await _context.Occurrences
                .Where(c => c.OrganizationId.Equals(id) && c.CreatedAt.Year == DateTime.Now.Year)
                .Include(o => o.Tp_Ocorrencia)
                .GroupBy(o => new {o.Tp_Ocorrencia.Name,o.CreatedAt.Month})
                .Select(x => new SummarryOccurrencePerName()
                {
                    Name = x.Key.Name,
                    Cost = x.Sum(i => i.Cost),
                    Month = x.Key.Month,
                })
                .ToListAsync();
        }
    }

}
