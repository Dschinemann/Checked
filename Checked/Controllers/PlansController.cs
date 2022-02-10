using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checked.Models.ViewModels;
using Checked.Data;
using Checked.Models.Models;
using Microsoft.AspNet.Identity;
using Checked.Models;
using System.Diagnostics;

namespace Checked.Controllers
{
    public class PlansController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public PlansController(CheckedDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id, int planId)    
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return NotFound();            
            
            if (planId != 0)
            {
                var actionPlan = await _context.Plans
                    .Where(c => c.Id == planId)
                    .FirstOrDefaultAsync();
                if(actionPlan != null)
                {
                    var actions = await _context.Actions.Where(c => c.PlanId == planId).ToListAsync();
                    PlanViewModel model = new PlanViewModel();
                    model.PlanId = actionPlan.Id;
                    model.Subject = actionPlan.Subject;
                    model.Objective = actionPlan.Objective;
                    model.Accountable = actionPlan.Accountable;
                    model.DeadLine = $"{actionPlan.Forecast}";
                    model.Actions = actions;
                    return View(model);
                }
            }
            if (id == 0) return NotFound();
            ViewBag.OccurrenceId = id;
            var plan = await _context.Plans
                .Where(c => c.OccurrenceId == id)
                .Where(c => c.organizationId == user.OrganizationId)
                .FirstOrDefaultAsync();
            if (plan == null)
            {
                Occurrence occurrence = await _context.Occurrences                    
                    .Where(c => c.Id == id)
                    .Select(c => new Occurrence() { Name = c.Name, Id = c.Id })
                    .FirstAsync();
                return RedirectToAction(nameof(Create), new { ocurrenceName = occurrence.Name, id = occurrence.Id });
            }
            else
            {
                var actions = await _context.Actions.Where(c => c.PlanId == plan.Id).ToListAsync();
                PlanViewModel model = new PlanViewModel();
                model.PlanId = plan.Id;
                model.Subject = plan.Subject;
                model.Objective = plan.Objective;
                model.Accountable = plan.Accountable;
                model.DeadLine = $"{plan.Forecast}";
                model.Actions = actions;                
                return View(model);
            }            
        }

        public IActionResult Create(string ocurrenceName, int id)
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
                plan.organizationId = (int)user.OrganizationId;

                _context.Add(plan);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = model.OccurrenceId });
                }catch(Exception ex)
                {
                    return RedirectToAction(nameof(Error), new {message = ex});
                }

            }
            return View(model);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message=message });
        }
    }
}
