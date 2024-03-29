﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Checked.Models.Types;
using Checked.Servicos.ControllerServices;
using Microsoft.AspNetCore.Authorization;
using Checked.Models;
using System.Diagnostics;
using Checked.Servicos.Email;
using System.Net.Mail;
using System.Net.Mime;

namespace Checked.Controllers
{
    public class ActionsController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly ActionsService _service;
        private readonly IMailService _mailService;

        public ActionsController(
            CheckedDbContext context,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            ActionsService service,
            IMailService mailService
            )
        {
            _context = context;
            _userManager = userManager;
            _service = service;
            _mailService = mailService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager
                .FindByIdAsync(User.Identity.GetUserId());
            var actions = await _service.GetAllAsync(user.OrganizationId);
            ViewBag.Resume = _service.CountPerStatus(actions);
            return View(actions);
        }


        // GET: Actions/Details/5
        public async Task<IActionResult> Details(string? actionId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return View(nameof(Error), new ErrorViewModel { Message = $"Id inválido" });
            }

            var action = await _context.Actions
                .Include(a => a.Plan)
                .Include(o => o.Who)
                .Include(a => a.TP_Status)
                .FirstOrDefaultAsync(m => m.Id.Equals(actionId));
            if (action == null)
            {
                return View(nameof(Error), new ErrorViewModel { Message = $"Não há ações com este Id: {actionId}" });
            }

            return View(action);
        }

        // GET: Actions/Create
        public async Task<IActionResult> Create(string planId, string occurrenceId)
        {
            var user = await _userManager
                .FindByIdAsync(User.Identity.GetUserId());
            var plan = await _context.Plans.FirstAsync(c => c.Id.Equals(planId));
            var users = await _context.Users.Where(o => o.OrganizationId.Equals(user.OrganizationId)).ToListAsync();

            CreateActionViewModel model = new CreateActionViewModel();
            model.PlanId = planId;
            model.OccurrenceId = occurrenceId;
            model.Id = Guid.NewGuid().ToString();
            model.StatusId = (int)TP_StatusEnum.Aberto;
            model.Goal = plan.Goal;
            model.Users = new SelectList(users, "Id", "Name", user.Id);
            return View(model);
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("What,Why,Where,WhoId,Init,Finish,How,HowMuch, PlanId, OccurrenceId, Id,StatusId, Goal")] CreateActionViewModel model, string planId)
        {
            if (model.PlanId != planId) return View(nameof(Error), new ErrorViewModel { Message = $"Id não localizado" });

            var user = await _userManager
                .FindByIdAsync(User.Identity.GetUserId());
            var users = await _context.Users
                .Where(o => o.OrganizationId.Equals(user.OrganizationId))
                .ToListAsync();
            var plan = await _context.Plans
                .FirstAsync(c => c.Id.Equals(model.PlanId));
            var occurrence = await _context.Occurrences
                .FirstOrDefaultAsync(c => c.Id.Equals(model.OccurrenceId));

            if (plan.organizationId != user.OrganizationId)
            {
                ModelState.AddModelError(string.Empty, "Id da Empresa difere do Id da empresa do Usuário");
            }

            if (ModelState.IsValid)
            {
                var emailWho = await _userManager.FindByIdAsync(model.WhoId);
                Models.Models.Action action = new Models.Models.Action();
                action.CreatedAt = DateTime.Now;
                action.UpdatedAt = DateTime.Now;
                action.How = model.How;
                action.Where = model.Where;
                action.PlanId = model.PlanId;
                action.Init = model.Init;
                action.Finish = model.NewFinish;
                action.NewFinish = model.NewFinish;
                action.HowMuch = model.HowMuch;
                action.What = model.What;
                action.WhoId = model.WhoId;
                action.Why = model.Why;
                action.TP_StatusId = ((int)TP_StatusEnum.Aberto);
                action.OccurrenceId = model.OccurrenceId;
                action.OrganizationId = user.OrganizationId;
                action.CreatedById = user.Id;
                _context.Add(action);
                string linkAction = Url.Action("Index", "Plans", new { id = model.PlanId }, Request.Scheme) ?? "";
                try
                {
                    await _context.SaveChangesAsync();
                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = emailWho.Email,
                        Subject = "Uma nova ação foi criada com seu email",
                        View = Message(linkAction, "Você foi adicionado como responsável por uma ação")
                    });
                }
                catch (Exception e)
                {
                    return View(nameof(Error), new ErrorViewModel { Message = $"Error code 10: {e.Message}" });
                }


                var existsActionOpen = await _context.Actions
                    .Where(c => c.OccurrenceId == model.OccurrenceId && c.TP_StatusId != ((int)TP_StatusEnum.Encerrado))
                    .AnyAsync();

                if (existsActionOpen)
                {
                    occurrence.StatusActions = "Existe Ações em Aberto";
                }
                else
                {
                    occurrence.StatusActions = "Todas as ações estão encerradas";
                }
                _context.Update(occurrence);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Plans", new { planId = model.PlanId });
            }
            model.Users = new SelectList(users, "Id", "Name", model.WhoId);
            return View(model);
        }

        // GET: Actions/Edit/5
        public async Task<IActionResult> Edit(string actionId, string OccurrenceId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return View(nameof(Error), new ErrorViewModel { Message = $"Id não localizado" });
            }

            var action = await _context.Actions
                .Include(c => c.TP_Status)
                .Include(c => c.Who)
                .Include(o => o.Plan)
                .Include(o => o.Who)
                .FirstOrDefaultAsync(c => c.Id == actionId);
            var users = await _context.Users
                .Where(c => c.OrganizationId.Equals(action.OrganizationId))
                .ToListAsync();

            if (action == null)
            {
                return View(nameof(Error), new ErrorViewModel { Message = $"Não existe ação com este Id:{actionId}" });
            }

            bool permitEdit = action.CreatedById.Equals(User.Identity.GetUserId()) | action.WhoId.Equals(User.Identity.GetUserId());
            if (!permitEdit)
            {
                ViewBag.Message = "Você não tem permissão para editar este conteúdo";
                return View("Info");
            }

            CreateActionViewModel model = new CreateActionViewModel()
            {
                Id = action.Id,
                How = action.How,
                HowMuch = action.HowMuch,
                PlanId = action.PlanId,
                What = action.What,
                Init = action.Init,
                Finish = action.Finish,
                NewFinish = action.NewFinish,
                Where = action.Where,
                Users = new SelectList(users, "Id", "Name", action.Who.Id),
                Why = action.Why,
                OccurrenceId = OccurrenceId,
                StatusId = action.TP_StatusId,
                Status = action.TP_Status,
                Goal = action.Plan.Goal
            };
            ViewBag.Status = new SelectList(_context.TP_Status, "Id", "Name", model.Status);
            return View(model);
        }

        // POST: Actions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string planId, [Bind("Id,What,Why,Where,WhoId,Init,Finish,NewFinish,How,HowMuch,PlanId,Status,OccurrenceId,Goal")] CreateActionViewModel model)
        {
            if (planId != model.PlanId)
            {
                return View(nameof(Error), new ErrorViewModel { Message = $"Id não localizado" });
            }
            var occurrence = await _context.Occurrences
                .FirstAsync(c => c.Id == model.OccurrenceId);
            var users = await _context.Users
                .Where(c => c.OrganizationId.Equals(occurrence.OrganizationId))
                .ToListAsync();
            if (ModelState.IsValid)
            {
                Models.Models.Action action = await _context.Actions.FirstAsync(c => c.Id.Equals(model.Id));
                var emailWho = await _userManager.FindByIdAsync(model.WhoId);

                action.Id = model.Id;
                action.What = model.What;
                action.Why = model.Why;
                action.Where = model.Where;
                action.WhoId = model.WhoId;
                action.Init = model.Init;
                action.Finish = model.Finish;
                action.NewFinish = model.NewFinish;
                action.How = model.How;
                action.HowMuch = model.HowMuch;
                action.UpdatedAt = DateTime.Now;
                action.PlanId = model.PlanId;
                action.TP_StatusId = model.Status.Id;
                action.OccurrenceId = model.OccurrenceId;
                action.OrganizationId = occurrence.OrganizationId;

                _context.Update(action);
                string linkAction = Url.Action("Index", "Plans", new { id = model.PlanId }, Request.Scheme) ?? "";
                try
                {
                    await _context.SaveChangesAsync();

                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = emailWho.Email,
                        Subject = "Uma nova ação foi atualizada com seu email",
                        View = Message(linkAction, "Você foi adicionado como responsável por uma ação")
                    });

                    var existsActionOpen = await _context.Actions
                    .Where(c => c.OccurrenceId.Equals(model.OccurrenceId) && c.TP_StatusId != ((int)TP_StatusEnum.Encerrado))
                    .AnyAsync();

                    if (existsActionOpen)
                    {
                        occurrence.StatusActions = "Existe Ações em Aberto";
                    }
                    else
                    {
                        occurrence.StatusActions = "Todas as ações estão encerradas";
                    }
                    _context.Update(occurrence);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Plans", new { planId = model.PlanId });
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!ActionExists(model.Id))
                    {
                        return View(nameof(Error), new ErrorViewModel { Message = $"Não existe ação com esse ID: {model.Id}" });
                    }
                    else
                    {
                        return View(nameof(Error), new ErrorViewModel { Message = $"Error code 30: {e.Message}" });
                    }
                }
            }

            ViewBag.Status = new SelectList(_context.TP_Status, "Id", "Name", model.Status);
            model.Users = new SelectList(users, "Id", "Name", model.WhoId);
            return View(model);
        }

        // GET: Actions/Delete/5
        public async Task<IActionResult> Delete(string? actionId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return View(nameof(Error), new { Message = "Não foi informado um Id válido" });
            }

            var action = await _context.Actions
                .Include(a => a.Plan)
                .Include(o => o.TP_Status)
                .FirstAsync(m => m.Id.Equals(actionId));
            if (action != null)
            {
                bool permitEdit = action.CreatedById.Equals(User.Identity.GetUserId()) | action.WhoId.Equals(User.Identity.GetUserId());
                if (!permitEdit)
                {
                    ViewBag.Message = "Você não tem permissão para editar este conteúdo";
                    return View("Info");
                }
                return View(action);
            }
            else
            {
                return View(nameof(Error), new { Message = "Não há ações para o plano cadastrado" });
            }
        }

        // POST: Actions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {            
            var action = await _context.Actions.FindAsync(Id);

            if (action != null)
            {
                bool permitEdit = action.CreatedById.Equals(User.Identity.GetUserId()) | action.WhoId.Equals(User.Identity.GetUserId());
                if (!permitEdit)
                {
                    ViewBag.Message = "Você não tem permissão para deletar este conteúdo";
                    return View("Info");
                }
                _context.Actions.Remove(action);
                await _context.SaveChangesAsync();

                bool existActions = await _context.Actions
                    .Where(c => c.OccurrenceId == action.OccurrenceId)
                    .AnyAsync();
                Occurrence oc = await _context.Occurrences
                        .FirstAsync(c => c.Id == action.OccurrenceId);

                var existsActionOpen = await _context.Actions
                    .Where(c => c.OccurrenceId == action.OccurrenceId && c.TP_StatusId != ((int)TP_StatusEnum.Encerrado))
                    .AnyAsync();

                if (existsActionOpen)
                {
                    oc.StatusActions = "Existe Ações em Aberto";
                }
                else
                {
                    oc.StatusActions = "Todas as ações estão encerradas";
                }
                if (!existActions)
                {
                    oc.StatusActions = "Não há Planos de ação para esta ocorrencia";
                }
                _context.Occurrences.Update(oc);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Plans", new { planId = action.PlanId });
            }
            return View();
        }

        private bool ActionExists(string id)
        {
            return _context.Actions.Any(e => e.Id == id);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult isValideFinishDate(string newFinish, string goal)
        {
            var min = DateTime.Now;
            var max = DateTime.Parse(goal);
            var msg = string.Format("O inicio e o fim para a ação deve estar entre {0:dd/MM/yyyy} e {1:dd/MM/yyyy}", min, max);
            try
            {
                var finishDate = DateTime.Parse(newFinish);
                if (finishDate <= max)
                {
                    return Json(true);
                }
                else
                {
                    return Json(msg);
                }

            }
            catch (Exception)
            {
                return Json(msg);
            }
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
                            Clique no <a href='{link}'>Link</a> para mais informações
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
    }
}
