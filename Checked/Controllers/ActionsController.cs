using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Checked.Models.Types;
using Checked.Servicos.ControllerServices;
using Microsoft.AspNetCore.Authorization;
using Checked.Models;
using System.Diagnostics;

namespace Checked.Controllers
{
    public class ActionsController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly ActionsService _service;
        public ActionsController(
            CheckedDbContext context,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            ActionsService service
            )
        {
            _context = context;
            _userManager = userManager;
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager
                .FindByIdAsync(User.Identity.GetUserId());
            var actions = await _service.GetAllAsync(user.OrganizationId);
            ViewBag.Resume = _service.CountPerStatus(actions);
            return View(actions);
        }


        // GET: Actions/Details/5
        public async Task<IActionResult> Details(string? actionId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return View(nameof(Error), new { Message = $"Id inválido" });
            }

            var action = await _context.Actions
                .Include(a => a.Plan)
                .FirstOrDefaultAsync(m => m.Id == actionId);
            if (action == null)
            {
                return View(nameof(Error), new { Message = $"Não há ações com este Id: {actionId}" });
            }

            return View(action);
        }

        // GET: Actions/Create
        public async Task<IActionResult> Create(string planId, string occurrenceId)
        {
            var plan = await _context.Plans.FirstAsync(c => c.Id.Equals(planId));
            CreateActionViewModel model = new CreateActionViewModel();
            model.PlanId = planId;
            model.OccurrenceId = occurrenceId;
            model.Id = Guid.NewGuid().ToString();
            model.StatusId = (int)TP_StatusEnum.Aberto;
            model.Goal = plan.Goal;
            return View(model);
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("What,Why,Where,Who,Init,Finish,How,HowMuch, PlanId, OccurrenceId, Id,StatusId, Goal")] CreateActionViewModel model, string planId)
        {
            if (model.PlanId != planId) return View(nameof(Error), new { Message = $"Id não localizado" });

            var user = await _userManager
                .FindByIdAsync(User.Identity.GetUserId());
            var plan = await _context.Plans
                .FirstOrDefaultAsync(c => c.Id == model.PlanId);
            var occurrence = await _context.Occurrences
                .FirstOrDefaultAsync(c => c.Id == model.OccurrenceId);

            if (plan.organizationId != user.OrganizationId)
            {
                ModelState.AddModelError(string.Empty, "Id da Empresa difere do Id da empresa do Usuário");
            }

            if (ModelState.IsValid)
            {
                Models.Models.Action action = new Models.Models.Action();
                action.CreatedAt = DateTime.Now;
                action.UpdatedAt = DateTime.Now;
                action.How = model.How;
                action.Where = model.Where;
                action.PlanId = model.PlanId;
                action.Init = model.Init;
                action.Finish = model.NewFinish;
                action.NewFinish = model.NewFinish;
                action.HowMuch = model.HowMuch;
                action.What = model.What;
                action.Who = model.Who;
                action.Why = model.Why;
                action.TP_StatusId = ((int)TP_StatusEnum.Aberto);
                action.OccurrenceId = model.OccurrenceId;
                action.OrganizationId = user.OrganizationId;
                _context.Add(action);
                await _context.SaveChangesAsync();

                var existsActionOpen = await _context.Actions
                    .Where(c => c.OccurrenceId == model.OccurrenceId && c.TP_StatusId != ((int)TP_StatusEnum.Encerrado))
                    .AnyAsync();

                if (existsActionOpen)
                {
                    occurrence.StatusActions = "Existe Ações em Aberto";
                }
                else
                {
                    occurrence.StatusActions = "Todas as ações estão encerradas";
                }
                _context.Update(occurrence);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Plans", new { planId = model.PlanId });
            }
            return View(model);
        }

        // GET: Actions/Edit/5
        public async Task<IActionResult> Edit(string actionId, string OccurrenceId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return View(nameof(Error), new { Message = $"Id não localizado" });
            }

            var action = await _context.Actions
                .Include(c => c.TP_Status)
                .Include(o => o.Plan)
                .FirstOrDefaultAsync(c => c.Id == actionId);

            if (action == null)
            {
                return View(nameof(Error), new { Message = $"Não existe ação com este Id:{actionId}" });
            }

            CreateActionViewModel model = new CreateActionViewModel()
            {
                Id = action.Id,
                How = action.How,
                HowMuch = action.HowMuch,
                PlanId = action.PlanId,
                What = action.What,
                Init = action.Init,
                Finish = action.Finish,
                NewFinish = action.NewFinish,
                Where = action.Where,
                Who = action.Who,
                Why = action.Why,
                OccurrenceId = OccurrenceId,
                StatusId = action.TP_StatusId,
                Status = action.TP_Status,
                Goal = action.Plan.Goal
            };
            ViewBag.Status = new SelectList(_context.TP_Status, "Id", "Name", model.Status);
            return View(model);
        }

        // POST: Actions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string planId, [Bind("Id,What,Why,Where,Who,Init,Finish,NewFinish,How,HowMuch,PlanId,Status,OccurrenceId,Goal")] CreateActionViewModel model)
        {
            if (planId != model.PlanId)
            {
                return View(nameof(Error), new { Message = $"Id não localizado" });
            }
            var occurrence = await _context.Occurrences
                .FirstOrDefaultAsync(c => c.Id == model.OccurrenceId);
            if (ModelState.IsValid)
            {
                try
                {
                    Models.Models.Action action = new Models.Models.Action()
                    {
                        Id = model.Id,
                        What = model.What,
                        Why = model.Why,
                        Where = model.Where,
                        Who = model.Who,
                        Init = model.Init,
                        Finish = model.Finish,
                        NewFinish = model.NewFinish,
                        How = model.How,
                        HowMuch = model.HowMuch,
                        UpdatedAt = DateTime.Now,
                        PlanId = model.PlanId,
                        TP_StatusId = model.Status.Id,
                        OccurrenceId = model.OccurrenceId,
                        OrganizationId = occurrence.OrganizationId
                    };
                    _context.Update(action);
                    await _context.SaveChangesAsync();

                    var existsActionOpen = await _context.Actions
                    .Where(c => c.OccurrenceId == model.OccurrenceId && c.TP_StatusId != ((int)TP_StatusEnum.Encerrado))
                    .AnyAsync();

                    if (existsActionOpen)
                    {
                        occurrence.StatusActions = "Existe Ações em Aberto";
                    }
                    else
                    {
                        occurrence.StatusActions = "Todas as ações estão encerradas";
                    }
                    _context.Update(occurrence);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Plans", new { planId = model.PlanId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionExists(model.Id))
                    {
                        return View(nameof(Error), new { Message = $"Não existe ação com esse ID: {model.Id}" });
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Plans", new { planId = model.PlanId });
            }
            return View(model);
        }

        // GET: Actions/Delete/5
        public async Task<IActionResult> Delete(string? actionId)
        {
            if (actionId == null)
            {
                return View(nameof(Error), new {Message = "Não foi informado um Id válido"});
            }

            var action = await _context.Actions
                .Include(a => a.Plan)
                .Include(o => o.TP_Status)
                .FirstOrDefaultAsync(m => m.Id == actionId);
            if (action == null)
            {
                return View(nameof(Error), new { Message = "Não há ações para o plano cadastrado" });
            }

            return View(action);
        }

        // POST: Actions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            var action = await _context.Actions.FindAsync(Id);

            if (action != null)
            {
                _context.Actions.Remove(action);
                await _context.SaveChangesAsync();

                bool existActions = await _context.Actions
                    .Where(c => c.OccurrenceId == action.OccurrenceId)
                    .AnyAsync();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Occurrence oc = await _context.Occurrences
                        .FirstOrDefaultAsync(c => c.Id == action.OccurrenceId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.


                var existsActionOpen = await _context.Actions
                    .Where(c => c.OccurrenceId == action.OccurrenceId && c.TP_StatusId != ((int)TP_StatusEnum.Encerrado))
                    .AnyAsync();

                if (existsActionOpen)
                {
                    oc.StatusActions = "Existe Ações em Aberto";
                }
                else
                {
                    oc.StatusActions = "Todas as ações estão encerradas";
                }
                if (!existActions)
                {
                    oc.StatusActions = "Não há Planos de ação para esta ocorrencia";
                }
                _context.Occurrences.Update(oc);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Plans", new { planId = action.PlanId });
            }
            return View();
        }

        private bool ActionExists(string id)
        {
            return _context.Actions.Any(e => e.Id == id);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult isValideFinishDate(string newFinish, string goal)
        {
            var min = DateTime.Now;
            var max = DateTime.Parse(goal);
            var msg = string.Format("O inicio e o fim para a ação deve estar entre {0:dd/MM/yyyy} e {1:dd/MM/yyyy}", min, max);
            try
            {
                var finishDate = DateTime.Parse(newFinish);
                if(finishDate <= max)
                {
                    return Json(true);
                }
                else
                {
                    return Json(msg);
                }
                
            }
            catch (Exception)
            {
                return Json(msg);
            }           
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
        }
    }
}
