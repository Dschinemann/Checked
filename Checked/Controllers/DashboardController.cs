using Checked.Models.Models;
using Checked.Models.ViewModels;
using Checked.Servicos;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Checked.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashService _service;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        public DashboardController(DashService service, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            OccurrenceResume ocResume = new OccurrenceResume();
            Organization organization = null;
            DashViewModel model = null;

            if (user == null) return NotFound();
            if (user.OrganizationId == null)
            {
                return RedirectToAction("Create", "Organizations");
            }
            if(user.OrganizationId != null)
            {
                ocResume = await _service.GetOccurrenceAsync(user.OrganizationId);
                organization = await _service.GetOrganizationAsync(user.OrganizationId); 
                var resume = await _service.GetCountActionsAsync(user.OrganizationId);
                var plansResume = await _service.GetCountPlansAsync(user.OrganizationId);
                var deadLine = await _service.GetDeadlineAsync(user.OrganizationId);
                model = new DashViewModel()
                {
                    OccurrenceResume = ocResume,
                    OrganizationName = organization.Name,
                    resume = resume,
                    PlanResume = plansResume,
                    DeadLineActions = deadLine
                };
            }
            return View(model);
        }
    }
}
