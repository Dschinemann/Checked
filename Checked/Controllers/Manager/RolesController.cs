using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Checked.Controllers.Manager
{
    /*[AllowAnonymous]
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(_roleManager.Roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;
                var roleExist = await _roleManager.RoleExistsAsync(name);
                if (!roleExist)
                {
                    result = await _roleManager.CreateAsync(new IdentityRole(name));
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", result.ToString());
                    }
                }
            }
            return View(name);
        }

    }
    */
}
