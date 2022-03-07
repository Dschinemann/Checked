using Checked.Data;
using Checked.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checked.Controllers
{
    public class MyTasksController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MyTasksController> _logger;

        public MyTasksController(CheckedDbContext context, UserManager<ApplicationUser> userManager, ILogger<MyTasksController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetOccurrencePerStatus(int status)
        {
            if (status != 0)
            {
                var user = await _userManager.GetUserAsync(User);
                var occurrence = await _context.Occurrences
                    .Where(c => c.OrganizationId.Equals(user.OrganizationId))
                    .Where(c => c.StatusId.Equals(status))
                    .ToListAsync();
                return Json(occurrence);
            }
            return Json(new { Error = "status da ocorrência é invalido" });
        }

        public async Task<IActionResult> GetPlansPerStatus(decimal initial, decimal final, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };
            List<Plan> plans = new List<Plan>();
            if (!string.IsNullOrEmpty(status) && status.Equals("finalizado"))
            {
                plans = await _context.Plans
                    .Include(o => o.Actions)
                    .Where(c => c.organizationId.Equals(user.OrganizationId))
                    .Where(c => c.AccountableId.Equals(user.Id))
                    .Where(c => !c.Actions.Any(c => !c.TP_StatusId.Equals(3) && !c.TP_StatusId.Equals(2)))
                    .ToListAsync();
                return Json(plans, options);
            }
            plans = await _context.Plans
                .Include(o => o.Actions)
                .Where(c => c.organizationId.Equals(user.OrganizationId))
                .Where(c => c.AccountableId.Equals(user.Id))
                .Where(c => c.Actions.Any(c => !c.TP_StatusId.Equals(3) && !c.TP_StatusId.Equals(2)))
                .ToListAsync();

            var json = plans.Where(c =>
            {
                var totalDeDiasDoPlano = (int)c.Goal.Subtract(c.CreatedAt).TotalDays;
                var prazo = (int)c.Goal.Subtract(DateTime.Today).TotalDays;
                var decorrido = (int)totalDeDiasDoPlano - prazo;
                var teste = (decorrido / totalDeDiasDoPlano);
                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("atrasado") && prazo <= 0) return true; else return false;
                }
                if (teste >= initial && teste < final) return true; else return false;
            }).ToList();
            return Json(json, options);
        }

        public async Task<IActionResult> GetActionsPerStatus(decimal initial, decimal final, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            List<Models.Models.Action> actions = new List<Models.Models.Action>();
            if (!string.IsNullOrEmpty(status) && status.Equals("finalizado"))
            {
                actions = await _context.Actions
                    .Where(c => c.OrganizationId.Equals(user.OrganizationId))
                    .Where(c => c.WhoId.Equals(user.Id))
                    .Where(c => c.TP_StatusId.Equals(3))
                    .Select(s => new Models.Models.Action()
                    {
                        What = s.What,
                        How = s.How,
                        Where = s.Where,
                        NewFinish = s.NewFinish,
                        Id = s.Id
                    })
                    .ToListAsync();

                return Json(actions);
            }
            actions = await _context.Actions
                    .Where(c => c.OrganizationId.Equals(user.OrganizationId))
                    .Where(c => c.WhoId.Equals(user.Id))
                    .Where(c => !c.TP_StatusId.Equals(3))
                    .Select(s => new Models.Models.Action()
                    {
                        What = s.What,
                        How = s.How,
                        Where = s.Where,
                        NewFinish = s.NewFinish,
                        Init = s.Init,
                        Id = s.Id
                    })
                    .ToListAsync();
            var json = actions.Where(c =>
            {
                var totalDeDiasDoPlano = (int)c.NewFinish.Subtract(c.Init).TotalDays == 0 ? 1 : (int)c.NewFinish.Subtract(c.Init).TotalDays;
                var prazo = (int)c.NewFinish.Subtract(DateTime.Today).TotalDays;
                var decorrido = (int)totalDeDiasDoPlano - prazo;
                var teste = (decorrido / totalDeDiasDoPlano);
                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("atrasado") && prazo <= 0) return true; else return false;
                }
                if (teste >= initial && teste < final) return true; else return false;
            }).ToList();
            return Json(json);
        }
    }
}
