using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Checked.Models.ViewModels;
using Checked.Models.Models;
using Checked.Servicos.Email;
using Checked.Models;
using Checked.Servicos.InviteService;
using System.Diagnostics;
using System.Text.Encodings.Web;


namespace Checked.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IMailService _mailService;
        private readonly InviteService _inviteservice;

        public AccountController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMailService mailService,
            InviteService service
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mailService = mailService;
            _inviteservice = service;
        }

        /*
         * Registrar um usuario
         */
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)

        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    CreatedAt = DateTime.Now,
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Country = model.Country,
                    Region = model.Region,
                    PostalCode = model.PostalCode,
                    City = model.City,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmAccount", "Account", new { userId = user.Id, token = code }, Request.Scheme);
                    try
                    {
                        await _mailService.SendEmailAsync(new EmailRequest()
                        {
                            ToEmail = model.Email,
                            Subject = "Email Confirm",
                            Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>."
                        });

                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                    }

                    ViewBag.Message = $"an email has been sent to {model.Email}, confirm your account before logging in.";
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return View("Info");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"O Email {email} já está sendo usado.");
            }
        }
        /*
         * LogOut
         */

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAccount(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Error = $"userId is not valid";
                return View(nameof(Error));
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = $"email is not confirmed {result.ToString}";
            return View(nameof(Error));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*
         * 
         *Forgot password 
         */
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email is not exists");
                    return View(model);
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var confirmationLink = Url.Action("ChangePassword", "Account", new { userId = user.Id, token = code }, Request.Scheme);
                try
                {
                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = model.Email,
                        Subject = "Forgot Password.",
                        Body = $"To change your password <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>click here</a>.<br/>" +
                        $"Or copy o link <br/><span style=\"font-weight:bold\"> {HtmlEncoder.Default.Encode(confirmationLink)}</span><br/> and paste into your favorite browser."
                    });

                }
                catch (Exception)
                {
                    throw;
                }
            }
            ViewBag.Message = $"A link has been sent to {model.Email}";
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult ChangePassword(string userId, string token)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordAsync(string userId, string token, ChangePass model)
        {
            if (userId == null || token == null) return RedirectToAction(nameof(Error));
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ViewBag.Error = "User not Found";
                    return RedirectToAction(nameof(Error));
                }
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (result.Succeeded)
                {
                    ViewBag.Message = "password successfully modified";
                    return View("info");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View();
        }
        /*
         * 
         * Invite new Users
         * 
         */
        public async Task<IActionResult> InviteUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.OrganizationId != null)
            {
                return View(new InviteViewModel { OrganizationId = (int)user.OrganizationId });
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteUserAsync([Bind("Email, OrganizationId")] InviteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string response = await _inviteservice.Invite(new Invite { Email = model.Email, OrganizationId = model.OrganizationId });

                    var inviteLink = Url.Action("CreateUser", "Account", new { organizationId = model.OrganizationId }, Request.Scheme);
                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = model.Email,
                        Subject = "Faça parte do meu grupo.",
                        Body = @$"Você foi convidado a partipar do meu grupo, <a href='{HtmlEncoder.Default.Encode(inviteLink)}'>clique no link para se cadastrar</a>.<br/>"
                    });
                    ModelState.AddModelError(string.Empty, response);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateUser(int organizationId)
        {
            RegisterViewModel model = new RegisterViewModel { organizationId = organizationId };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserAsync([Bind("Name,Email,Country,Region,City,PostalCode,Password,ConfirmPassword,organizationId")] RegisterViewModel model)
        {
            if(model.organizationId == 0)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    CreatedAt = DateTime.Now,
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Country = model.Country,
                    Region = model.Region,
                    PostalCode = model.PostalCode,
                    City = model.City,
                    OrganizationId = model.organizationId,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Home",new LoginViewModel { Email = model.Email, Password = model.Password});
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

    }
}
