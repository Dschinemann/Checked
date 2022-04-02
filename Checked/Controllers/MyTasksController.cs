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

            var keyValuePairs = await _service.GetOccurrencesAsync(user);
            return View(keyValuePairs);
        }

        public async Task<IActionResult> GetTaskPerType(string? type)
        {
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(type))
            {
                var keyValuePairs = await _service.GetOccurrencesAsync(user);
                return Json(keyValuePairs);
            }
            if (type.Equals("plans"))
            {
                var keyValuePairs = await _service.GetPlansAsync(user);
                return Json(keyValuePairs);
            }
            if (type.Equals("actions"))
            {
                var keyValuePairs = await _service.GetActionsAsync(user);
                return Json(keyValuePairs);
            }
            Dictionary<string, List<string>> errorPair = new Dictionary<string, List<string>>();
            List<string> obj = new List<string>();
            return Json(new {Error = "Sem registros"});
        }
        
    }
}
