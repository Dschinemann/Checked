using Checked.Data;
using Checked.Models.Models;
using Checked.Servicos.ControllerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            if (type.Equals("others"))
            {
                var keyValuePairs = await _service.GetPersonTasks(user);
                return Json(keyValuePairs);
            }
            Dictionary<string, List<string>> errorPair = new Dictionary<string, List<string>>();
            List<string> obj = new List<string>();
            return Json(new {Error = "Sem registros"});
        }
        
        [HttpPost,ActionName("createOrUpdate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CreateOrUpdateTask([Bind("Title,Description,Id,StatusID,UserId")] Models.Models.Task task)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!string.IsNullOrEmpty(task.Id))
            {
                var resultTask = await _context.Tasks.AnyAsync(c => c.Id.Equals(task.Id));
                if(resultTask)
                {
                    var updateTask = await _context.Tasks.Where(c => c.Id.Equals(task.Id)).FirstAsync();
                    updateTask.Description = task.Description?? updateTask.Description;
                    updateTask.Title = task.Title?? updateTask.Title;
                    updateTask.StatusID = task.StatusID;
                    _context.Tasks.Update(updateTask);
                    await _context.SaveChangesAsync();

                    var keyValuePairs = await _service.GetPersonTasks(user);
                    return Json(keyValuePairs);
                }
            }
            
            if(string.IsNullOrEmpty(task.Description) || string.IsNullOrEmpty(task.Title))
            {
                return Json(new { Error = "Campo Titulo ou Descrição esta vazio" });
            }

            try
            {
                task.Id = Guid.NewGuid().ToString();
                task.UserId = user.Id;
                task.StatusID = 2; //Pendente                 
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                var keyValuePairs = await _service.GetPersonTasks(user);
                return Json(keyValuePairs);
                
            }catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }

        }


        [HttpPost]  
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks.FindAsync(id);
            if(task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            var keyValuePairs = await _service.GetPersonTasks(user);
            return Json(keyValuePairs);
        }
    }
}
