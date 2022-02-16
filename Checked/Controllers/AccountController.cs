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
using Checked.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Checked.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IMailService _mailService;
        private readonly InviteService _inviteservice;
        private readonly CheckedDbContext _context;

        public AccountController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMailService mailService,
            InviteService service,
            CheckedDbContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mailService = mailService;
            _inviteservice = service;
            _context = context;
        }

        /*
         * Registrar um usuario
         */
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Pais = new SelectList(_context.Countries, "Id", "Pais", "Brasil");
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetStates(int idCountry)
        {
            try
            {
                var estados = await _context.States.Where(c => c.CountryId == idCountry).ToListAsync();
                return Json(estados);
            }
            catch (Exception e)
            {
                return Json(new { erro = e.Message });
            }

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetCityes(int IdState)
        {
            try
            {
                var cityes = await _context.Cities.Where(c => c.StateId == IdState).ToListAsync();
                return Json(cityes);
            }
            catch (Exception e)
            {
                return Json(new { erro = e.Message });
            }

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
                    CountryId = model.CountryId,
                    StateId = model.StateId,
                    PostalCode = model.PostalCode,
                    CityId = model.CityId,
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
                List<ApplicationUser> users = await _inviteservice.GetUsersAsync(user.OrganizationId);
                return View(new InviteViewModel { OrganizationId = user.OrganizationId, users = users });
            }
            else
            {
                return RedirectToAction("Create", "Organizations");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteUserAsync([Bind("Email, OrganizationId, Message")] InviteViewModel model)
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
                        Body = @$"Você foi convidado a partipar do meu grupo, <a href='{HtmlEncoder.Default.Encode(inviteLink)}'>clique no link para se cadastrar</a>.<br/>
                        <div><span>Message</span>{model.Message}</div>"
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
        public IActionResult CreateUser(string organizationId)
        {
            RegisterViewModel model = new RegisterViewModel { organizationId = organizationId };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserAsync([Bind("Name,Email,CountryId,StateId,CityId,PostalCode,Password,ConfirmPassword,organizationId")] RegisterViewModel model)
        {
            if (model.organizationId == null || model.organizationId == "")
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
                    CountryId = model.CountryId,
                    StateId = model.StateId,
                    PostalCode = model.PostalCode,
                    CityId = model.CityId,
                    OrganizationId = model.organizationId,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Home", new LoginViewModel { Email = model.Email, Password = model.Password });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        /*
         * 
         * ControlUsers
         * 
         */

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _context.Users
                .Include(o => o.City)
                .Include(o => o.Country)
                .Include(o => o.State)
                .Where(c => c.Id == userId)
                .FirstAsync();


            EditUserViewModel model = new EditUserViewModel()
            {
                Cities = await _context.Cities.Where(c => c.StateId == user.StateId).ToListAsync(),
                CityId = user.CityId,
                Country = user.Country,
                CountryId = user.CountryId,
                Id = user.Id,
                Name = user.Name,
                PostalCode = user.PostalCode,
                States = await _context.States.Where(c => c.CountryId == user.CountryId).ToListAsync(),
                StateId = user.StateId,
                City = user.City,
                State = user.State
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Cities,Country,States,City,State,Name,CountryId,StateId,CityId,PostalCode")] EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                user.Name = model.Name;
                user.CountryId = model.CountryId;
                user.StateId = model.StateId;
                user.CityId = model.CityId;
                user.PostalCode = model.PostalCode;
                try
                {
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(InviteUser));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }
            model.Country = await _context.Countries.Where(c => c.Id == model.CountryId).FirstAsync();
            model.City = await _context.Cities.FirstAsync(c => c.Id == model.CityId);
            model.State = await _context.States.FirstAsync(c => c.Id == model.StateId);
            model.Cities = await _context.Cities.Where(c => c.StateId == model.StateId).ToListAsync();
            model.States = await _context.States.Where(c => c.CountryId == model.CountryId).ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string userId)
        {
            var user = await _context.Users
                .Include(o => o.City)
                .Include(o => o.Country)
                .Include(o => o.State)
                .Where(c => c.Id == userId)
                .FirstAsync();
            EditUserViewModel model = new()
            {
                City = user.City,
                Country = user.Country,
                Name = user.Name,
                Id = user.Id,
                PostalCode = user.PostalCode,
                State = user.State
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            RegisterViewModel model = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("Email,Id,Name")] RegisterViewModel model, string userId)
        {
            if (userId == null) return NotFound();
            if (await UserExistsAsync(userId))
            {
                var user = await _context.Users.FirstAsync(c => c.Id.Equals(userId));
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(InviteUser));
            }
            ModelState.AddModelError(string.Empty, "Não foi possível excluir o usuário");
            return View(model);
        }
        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(c => c.Id.Equals(userId));
        }
    }

}
