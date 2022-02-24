using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checked.Models.ViewModels;
using Checked.Data;
using Checked.Models.Models;
using Microsoft.AspNet.Identity;
using Checked.Models;
using System.Diagnostics;
using Checked.Servicos.ControllerServices;
using Microsoft.AspNetCore.Authorization;

namespace Checked.Controllers
{
    public class PlansController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly PlansService _service;

        public PlansController(
            CheckedDbContext context,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            PlansService service
            )
        {
            _context = context;
            _userManager = userManager;
            _service = service;
        }
        public async Task<IActionResult> Plans()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return NotFound();
            var plans = await _service.GetPlansAsync(user.OrganizationId);
            return View(plans);
        }
        public async Task<IActionResult> Index(string? id, string? planId)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return NotFound();

            var actionPlan = await _context.Plans
                .Where(c => c.Id == planId)
                .FirstOrDefaultAsync();
            if (actionPlan != null)
            {
                var actions = await _context.Actions
                    .Where(c => c.PlanId == planId)
                    .Include(o => o.TP_Status)
                    .ToListAsync();

                PlanViewModel model = new PlanViewModel();
                model.PlanId = actionPlan.Id;
                model.Subject = actionPlan.Subject;
                model.Objective = actionPlan.Objective;
                model.Accountable = actionPlan.Accountable;
                model.DeadLine = $"{actionPlan.Forecast}";
                model.Actions = actions;
                model.OccurrenceId = actionPlan.OccurrenceId;
                model.Goal = actionPlan.Goal;
                model.QuantStatus = _service.GetSumActionPerStatus(actions);
                return View(model);
            }

            if (id == null || id == "") return NotFound();
            //ViewBag.OccurrenceId = id;
            var plan = await _context.Plans
                .Where(c => c.OccurrenceId == id)
                .Where(c => c.organizationId == user.OrganizationId)
                .FirstOrDefaultAsync();
            if (plan == null)
            {
                Occurrence occurrence = await _context.Occurrences
                    .Where(c => c.Id == id)
                    .Include(o => o.Tp_Ocorrencia)
                    .Select(c => new Occurrence() { Tp_Ocorrencia = c.Tp_Ocorrencia, Id = c.Id })
                    .FirstAsync();
                return RedirectToAction(nameof(Create), new { ocurrenceName = occurrence.Tp_Ocorrencia.Name, id = occurrence.Id });
            }
            else
            {
                var actions = await _context.Actions
                    .Where(c => c.PlanId == plan.Id)
                    .Include(o => o.TP_Status)
                    .ToListAsync();
                PlanViewModel model = new PlanViewModel();
                model.PlanId = plan.Id;
                model.Subject = plan.Subject;
                model.Objective = plan.Objective;
                model.Accountable = plan.Accountable;
                model.DeadLine = $"{plan.Forecast}";
                model.Actions = actions;
                model.OccurrenceId = id;
                model.Goal = plan.Goal;
                model.QuantStatus = _service.GetSumActionPerStatus(actions);
                return View(model);
            }
        }

        public IActionResult Create(string ocurrenceName, string id)
        {
            CreatePlanViewModel model = new CreatePlanViewModel();
            model.OccurrenceId = id;
            @ViewData["Occurrence"] = ocurrenceName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subject,Goal,Accountable,Objective,OccurrenceId")] CreatePlanViewModel model)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                Plan plan = new Plan();
                plan.OccurrenceId = model.OccurrenceId;
                plan.Goal = model.Goal;
                plan.Subject = model.Subject;
                plan.Accountable = model.Accountable;
                plan.Objective = model.Objective;
                plan.organizationId = user.OrganizationId;

                _context.Add(plan);
                var oc = await _context.Occurrences.FirstAsync(c => c.Id.Equals(plan.OccurrenceId));
                oc.PlanId = plan.Id;
                _context.Occurrences.Update(oc);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = model.OccurrenceId });
                }
                catch (Exception ex)
                {
                    return RedirectToAction(nameof(Error), new { message = ex });
                }

            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string? planId)
        {
            if (planId != null)
            {
                if (planId.Equals("")) return NotFound();

                var plan = await _service.GetPlanById(planId);
                CreatePlanViewModel model = new CreatePlanViewModel
                {
                    Accountable = plan.Accountable,
                    Objective = plan.Objective,
                    Goal = plan.Goal,
                    OccurrenceId = plan.OccurrenceId,
                    Subject = plan.Subject,
                    PlanId = plan.Id,
                    OrganizatioId = plan.organizationId
                };
                var name = await _context.Occurrences
                    .Include(o => o.Tp_Ocorrencia)
                    .FirstAsync(c => c.Id.Equals(plan.OccurrenceId));
                ViewData["Occurrence"] = name.Tp_Ocorrencia.Name;
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("OccurrenceId,Subject,Goal,Accountable,Objective, PlanId,OrganizatioId")] CreatePlanViewModel model, string PlanId)
        {
            if (model.PlanId != model.PlanId) return RedirectToAction(nameof(Error),"Id não conferem");
            if (ModelState.IsValid)
            {
                try
                {
                    Plan plan = new Plan()
                    {
                        Id = model.PlanId,
                        Objective = model.Objective,
                        Accountable = model.Accountable,
                        Goal = model.Goal,
                        OccurrenceId = model.OccurrenceId,
                        Subject = model.Subject,
                        organizationId = model.OrganizatioId
                    };
                    await _service.UpdatePlanAsync(plan);
                    return RedirectToAction(nameof(Plans));
                }
                catch (Exception ex)
                {
                    return RedirectToAction(nameof(Error), new { ex.Message });
                }
            }
            var name = await _context.Occurrences
                    .Include(o => o.Tp_Ocorrencia)
                    .FirstAsync(c => c.Id.Equals(model.OccurrenceId));
            ViewData["Occurrence"] = name.Tp_Ocorrencia.Name;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
        }

        public async Task<IActionResult>Delete(string planId)
        {
            if (string.IsNullOrEmpty(planId)) return NotFound();
            var model = await _context.Plans.FirstAsync(c => c.Id.Equals(planId));
            ViewBag.actions = await _context.Actions
                .Include(o => o.TP_Status)
                .Where(c => c.PlanId.Equals(model.Id))
                .ToListAsync();
            return View(model);
        }

        // POST: Actions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if(string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Error), new {message = "Não foi informado um ID válido"});
            var plan = await _context.Plans.FirstAsync(c => c.Id.Equals(id));
            var  actions = await _context.Actions.Where(c => c.PlanId.Equals(plan.Id)).ToListAsync();
            if(plan != null)
            {
                if(actions != null)
                {
                    try
                    {
                        _context.Actions.RemoveRange(actions);
                        await _context.SaveChangesAsync();
                    }catch(Exception ex)
                    {
                        return RedirectToAction(nameof(Error), new { message = ex.Message });
                    }
                }
                try
                {
                    plan = await _context.Plans.FirstAsync(c => c.Id.Equals(id));
                    _context.Plans.Remove(plan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Plans));
                }catch(Exception e)
                {
                    return RedirectToAction(nameof(Error), new { message = e.Message });
                }

            };
            return View();
        }
    }
}
