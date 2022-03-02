using Microsoft.AspNetCore.Mvc;

namespace Checked.Controllers
{
    public class MyTasksController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
