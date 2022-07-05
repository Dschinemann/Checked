using Checked.Models;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Checked.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index","Dashboard");
            }
            return new PartialViewResult()
            {
                ViewName = "_HomePartial"
            };            
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Terms()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_HomePartial", model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ViewBag.Message = "You must have a confirmed email to log on.";
                    return View("Info");
                }
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if(user.OrganizationId == null)
                {
                    ViewBag.ShowButtons = "false";
                    return RedirectToAction("Create", "Organizations");
                }
                return RedirectToAction("Index","Occurrences");
               
            }
            ModelState.AddModelError(String.Empty, result.ToString() + ": Login invalid");
            return PartialView("_HomePartial",model);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}