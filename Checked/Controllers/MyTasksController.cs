using Checked.Data;
using Checked.Models.Models;
using Checked.Models.Types;
using Checked.Models.ViewModels;
using Checked.Servicos.ControllerServices;
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
        private readonly TaskService _service;

        public MyTasksController(CheckedDbContext context, UserManager<ApplicationUser> userManager, TaskService service)
        {
            _context = context;
            _userManager = userManager;
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var oc = await _service.GetOccurrencesAsync(user.OrganizationId, user.Id);
            var pl = await _service.GetPlansStatusOpenAsync(user.OrganizationId, user.Id);
            var ac = await _service.GetActionsStatusOpenAsync(user.OrganizationId, user.Id);
            var PlFinish = await _service.GetPlansStatusFinishAsync(user.OrganizationId, user.Id);
            var AcFinish = await _service.GetActionsStatusFinishAsync(user.OrganizationId, user.Id);

            TaskViewModel model = new TaskViewModel()
            {
                OcEmAnalise = oc.Count(o => o.StatusId == ((int)TP_StatusOccurenceEnum.EmAnalise)),
                OcNãoProcedente = oc.Count(o => o.StatusId == ((int)TP_StatusOccurenceEnum.Nao_Procedente)),
                OcNaoIdentificado = oc.Count(o => o.StatusId == ((int)TP_StatusOccurenceEnum.Nao_Identificao)),
                OcProcedente = oc.Count(o => o.StatusId == ((int)TP_StatusOccurenceEnum.Procedente)),

                /*  Plans   */
                PlAtrasado = pl.Count(o =>
                {
                    var totalDeDiasDoPlano = (int)o.Goal.Subtract(o.CreatedAt).TotalDays == 0 ? 1 : (int)o.Goal.Subtract(o.CreatedAt).TotalDays;
                    var prazo = (int)o.Goal.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano);
                    if (prazo <= 0) return true; else return false;
                }),
                PlEmTempo = pl.Count(o =>
                {
                    var totalDeDiasDoPlano = (int)o.Goal.Subtract(o.CreatedAt).TotalDays == 0 ? 1 : (int)o.Goal.Subtract(o.CreatedAt).TotalDays;
                    var prazo = (int)o.Goal.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano);
                    if (shelf >= 0 && shelf < 0.5) return true; else return false;
                }),
                PlFinalizado = PlFinish.Count(),

                PlProxVencimento = pl.Count(o =>
                {
                    var totalDeDiasDoPlano = (int)o.Goal.Subtract(o.CreatedAt).TotalDays == 0 ? 1 : (int)o.Goal.Subtract(o.CreatedAt).TotalDays;
                    var prazo = (int)o.Goal.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano);
                    if (shelf >= 0.5 && shelf < 1) return true; else return false;
                }),

                /* Actions */
                AcAtrasado = ac.Count(o =>
                {
                    var totalDeDiasDoPlano = (int)o.NewFinish.Subtract(o.Init).TotalDays == 0 ? 1 : (int)o.NewFinish.Subtract(o.Init).TotalDays;
                    var prazo = (int)o.NewFinish.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano);
                    if (prazo <= 0) return true; else return false;
                }),

                AcEmTempo = ac.Count(o =>
                {
                    var totalDeDiasDoPlano = (int)o.NewFinish.Subtract(o.Init).TotalDays == 0 ? 1 : (int)o.NewFinish.Subtract(o.Init).TotalDays;
                    var prazo = (int)o.NewFinish.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano);
                    if (shelf >= 0 && shelf < 0.5) return true; else return false;
                }),

                AcProxVencimento = ac.Count(o =>
                {
                    var totalDeDiasDoPlano = (int)o.NewFinish.Subtract(o.Init).TotalDays == 0 ? 1 : (int)o.NewFinish.Subtract(o.Init).TotalDays;
                    var prazo = (int)o.NewFinish.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano);
                    if (shelf >= 0.5 && shelf < 1) return true; else return false;
                }),

                AcFinalizado = AcFinish.Count()

            };

            return View(model);
        }

        public async Task<IActionResult> GetOccurrencePerStatus(int status)
        {
            if (status != 0)
            {
                var user = await _userManager.GetUserAsync(User);
                var occurrence = await _context.Occurrences
                    .Where(c => c.OrganizationId.Equals(user.OrganizationId))
                    .Where(c => c.AppraiserId.Equals(user.Id))
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
                    .Where(c => c.Actions.Any(c => c.TP_StatusId.Equals(3) && c.TP_StatusId.Equals(2)))
                    .ToListAsync();
                return Json(plans, options);
            }
            plans = await _context.Plans
                .Include(o => o.Actions)
                .Where(c => c.organizationId.Equals(user.OrganizationId))
                .Where(c => c.AccountableId.Equals(user.Id))
                .Where(c => !c.Actions.Any(c => c.TP_StatusId.Equals(3) && c.TP_StatusId.Equals(2)))
                .ToListAsync();

            var json = plans.Where(c =>
            {
                var totalDeDiasDoPlano = (int)c.Goal.Subtract(c.CreatedAt).TotalDays == 0 ? 1 : (int)c.Goal.Subtract(c.CreatedAt).TotalDays;
                var prazo = (int)c.Goal.Subtract(DateTime.Today).TotalDays;
                var decorrido = (int)totalDeDiasDoPlano - prazo;
                var shelf = (decorrido / totalDeDiasDoPlano);
                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("atrasado") && prazo <= 0) return true; else return false;
                }
                if (shelf >= initial && shelf < final) return true; else return false;
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
                var shelf = (decorrido / totalDeDiasDoPlano);
                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("atrasado") && prazo <= 0) return true; else return false;
                }
                if (shelf >= initial && shelf < final) return true; else return false;
            }).ToList();
            return Json(json);
        }
    }
}
