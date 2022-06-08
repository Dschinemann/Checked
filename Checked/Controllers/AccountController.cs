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
using System.Net.Mail;
using System.Net.Mime;

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
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMailService mailService,
            InviteService service,
            CheckedDbContext context,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mailService = mailService;
            _inviteservice = service;
            _context = context;
            _roleManager = roleManager;
        }

        /*
         * Registrar um usuario
         */
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Pais = new SelectList(_context.Countries, "Id", "Pais", "Brasil");
            return View(new RegisterViewModel { StateName = "Selecione um estado", CityName = "Selecione uma cidade" });
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
                    var role = await _roleManager.FindByNameAsync("Administrador");
                    if (role != null)
                    {
                        var userSaved = await _userManager.FindByEmailAsync(model.Email);
                        if (userSaved != null)
                        {
                            IdentityResult res = await _userManager.AddToRoleAsync(userSaved, role.Name);
                            if (!res.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, "Não foi possivel adicionar um papel para usuario, adicionar manualmente");
                            }
                        }
                    }
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmationLink = Url.Action("ConfirmAccount", "Account", new { userId = user.Id, token = code }, Request.Scheme) ?? "";//$"{Ip}/Account/ConfirmAccount?userId={user.Id}&token={code}";//Url.Action("ConfirmAccount", "Account", new { userId = user.Id, token = code }, Request.Scheme) ?? "";
                    try
                    {
                        await _mailService.SendEmailAsync(new EmailRequest()
                        {
                            ToEmail = model.Email,
                            Subject = "Email Confirm",
                            View = AlternateView.CreateAlternateViewFromString(@$"Clique no <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>Link</a> para confirmar", null, MediaTypeNames.Text.Html),
                            //Body = @$"Clique no <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>Link</a> para confirmar"
                        });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e.Message);
                    }

                    ViewBag.Message = $"Para Confirmar sua conta clique no link enviado para {model.Email}.";
                    return View("Info");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var cityName = await _context.Cities.FindAsync(model.CityId);
            var stateName = await _context.States.FindAsync(model.StateId);
            model.CityName = cityName == null ? "Selecione uma cidade" : cityName.Name;
            model.StateName = stateName == null ? "Selecione um estado" : stateName.Name;
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
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
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
                string confirmationLink = Url.Action("ChangePassword", "Account", new { userId = user.Id, token = code }, Request.Scheme) ?? "";  //$"{Ip}/Account/ChangePassword?userId={user.Id}&token={code}";//Url.Action("ChangePassword", "Account", new { userId = user.Id, token = code }, Request.Scheme) ?? "";
                try
                {
                    string message = $"To change your password <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>click here</a>.<br/>" +
                    $"Or copy o link <br/><span style=\"font-weight:bold\"> {HtmlEncoder.Default.Encode(confirmationLink)}</span><br/> and paste into your favorite browser.";
                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = model.Email,
                        Subject = "Forgot Password.",
                        View = AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html)
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InviteUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.OrganizationId != null)
            {
                List<UsersInRoleViewModel> users = await _inviteservice
                    .GetUsersAsync(user.OrganizationId, user.Id);
                return View(new InviteViewModel { OrganizationId = user.OrganizationId, users = users });
            }
            else
            {
                return RedirectToAction("Create", "Organizations");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InviteUserAsync([Bind("Email, OrganizationId, Message, Name")] InviteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    string response = await _inviteservice.Invite(
                        new Invite
                        {
                            Email = model.Email,
                            OrganizationId = model.OrganizationId,
                            CreatedById = user.Id,
                            Name = model.Name
                        });
                    var org = await _context.Organizations.FirstAsync(c => c.Id.Equals(user.OrganizationId));
                    string inviteLink = Url.Action("CreateUser", "Account", new { organizationId = model.OrganizationId }, Request.Scheme) ?? ""; //$"{Ip}/Account/CreateUser?organizationId={model.OrganizationId}"; //Url.Action("CreateUser", "Account", new { organizationId = model.OrganizationId }, Request.Scheme) ?? "";
                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = model.Email,
                        Subject = "Faça parte do meu grupo.",
                        View = Message(inviteLink, @$"Você recebeu um convite de {user.Name} para ingressar no grupo {org.Name}")
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
            RegisterViewModel model = new RegisterViewModel { organizationId = organizationId, CityName = "Selecione uma cidade", StateName = "Selecione um estado" };
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
                    var role = await _roleManager.FindByNameAsync("Usuário");
                    var userSaved = await _userManager.FindByEmailAsync(model.Email);
                    if (userSaved != null)
                    {
                        IdentityResult res = await _userManager.AddToRoleAsync(userSaved, role.Name);
                        if (!res.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, "Não foi possivel adicionar um papel para usuario, adicionar manualmente");
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var cityName = await _context.Cities.FindAsync(model.CityId);
            var stateName = await _context.States.FindAsync(model.StateId);
            model.CityName = cityName == null ? "Selecione uma cidade" : cityName.Name;
            model.StateName = stateName == null ? "Selecione um estado" : stateName.Name;
            return View(model);
        }

        /*
         * 
         * ControlUsers
         * 
         */

        [HttpGet]
        [Authorize(Roles = "Administrador")]
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
            var roleId = await _context.UserRoles.Where(c => c.UserId.Equals(userId)).FirstAsync();
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name", roleId.RoleId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit([Bind("Id,Cities,Country,States,City,State,Name,CountryId,StateId,CityId,PostalCode,RoleId")] EditUserViewModel model)
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
                    // remove userRoles
                    var userRole = await _context.UserRoles.Where(c => c.UserId == model.Id).FirstAsync();
                    var userRoleName = await _context.Roles.FirstAsync(c => c.Id.Equals(userRole.RoleId));
                    await _userManager.RemoveFromRoleAsync(user, userRoleName.Name);
                    // add new role
                    var newUserRoleName = await _context.Roles.FirstAsync(c => c.Id.Equals(model.RoleId));
                    await _userManager.AddToRoleAsync(user, newUserRoleName.Name);
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
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name", await _context.UserRoles.Where(c => c.UserId == model.Id).ToListAsync());
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
        }

        private AlternateView Message(string link, string title)
        {
            var path = Path.GetFullPath("wwwroot");
            string message = @$"
            <!DOCTYPE html>
            <html lang='pt-BR'>
            <head>
                <meta charset='utf-8'>
                <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                <title>Teste</title>
                <meta name='viewport' content='width=device-width, initial-scale=1'>                
            </head>
            
            <body>
                <table style='width: 600px;padding: 10px;'>
                    <tr>
                        <td>
                             <img src='cid:Pic1'>
                        </td>
                    </tr>
                    <tr>
                        <td style='font-family:Verdana, Geneva, Tahoma, sans-serif;color: #1b29b6;text-align: center; font-size: 28px; font-weight: bold;'>
                            {title}
                        </td>
                    </tr>
                    <tr style='margin-top: 20px;'>
                        <td
                            style='font-size: 20px; font-family:Verdana, Geneva, Tahoma, sans-serif; color: #048162;text-align: center;padding: 16px;'>
                            Clique no <a href='{link}'>Link</a> para se cadastrar.
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html);
            LinkedResource pic1 = new LinkedResource($"{path}/css/Images/header.png", MediaTypeNames.Image.Jpeg);
            pic1.ContentId = "Pic1";
            alternateView.LinkedResources.Add(pic1);
            return alternateView;
        }

        public async Task<JsonResult> GetUsersPerOrganization(string organizationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _context.Users.Where(c => c.OrganizationId.Equals(user.OrganizationId)).Select(c => new ApplicationUser()
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return Json(result);
        }
    }

}
