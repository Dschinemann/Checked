using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Checked.Models;
using System.Diagnostics;
using Checked.Servicos.Email;
using System.Net.Mail;
using System.Net.Mime;
using Checked.Models.Types;
using Checked.Models.FilterModels;
using Checked.Servicos.ControllerServices;
using Checked.Servicos;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Checked.Controllers
{
    public class OccurrencesController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;
        private readonly OccurrenceService _service;

        public OccurrencesController(CheckedDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IMailService mailService, OccurrenceService service)
        {
            _context = context;
            _userManager = userManager;
            _mailService = mailService;
            _service = service;
        }

        // GET: Occurrences
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, string stringQuery)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            StringBuilder sql;
            string stringFilter;

            var urlParams = HttpContext.Request.Query;
            Dictionary<string, string> values = urlParams.ToDictionary(x => x.Key, x => x.Value.ToString());
                sql = _service.SqlConsult(values);
                stringFilter = _service.UrlCurrentFilter(values);
                       
            ViewData["CurrentFilter"] = stringFilter;

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.OrganizationId == null)
            {
                return RedirectToAction("Create", "Organizations");
            }
            var occurrences = _context.Occurrences
                .FromSqlRaw($"SELECT * FROM \"Occurrences\" AS A where 1=1 {sql}")
                .Where(c => c.OrganizationId == user.OrganizationId)
                .Include(o => o.Appraiser)
                .Include(o => o.Tp_Ocorrencia)
                .Include(o => o.Status)
                .Include(o => o.OcurrenceComplementList)
                .OrderByDescending(c => c.CreatedAt)
                .Select(o => new Occurrence()
                {
                    CreatedAt = o.CreatedAt,
                    DateOccurrence = o.DateOccurrence,
                    Description = o.Description,
                    Additional1 = o.Additional1,
                    Additional2 = o.Additional2,
                    Harmed = o.Harmed,
                    Document = o.Document,
                    Cost = o.Cost,
                    Appraiser = o.Appraiser,
                    Origin = o.Origin,
                    Id = o.Id,
                    OcurrenceComplementList = o.OcurrenceComplementList,
                    Tp_Ocorrencia = o.Tp_Ocorrencia,
                    Status = o.Status,
                    StatusActions = o.StatusActions,
                    CorrectiveAction = o.CorrectiveAction
                });

            int pageSize = 10;
            return View(await PaginatedList<Occurrence>.CreateAsync(occurrences.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Occurrences/Details/5
        public async Task<IActionResult> Details(string? idOccurrence)
        {
            if (idOccurrence == null || idOccurrence.Equals(""))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var occurrence = await _context.Occurrences
                .Where(c => c.OrganizationId == user.OrganizationId && c.Id == idOccurrence)
                .Include(o => o.ApplicationUser)
                .Include(o => o.Appraiser)
                .Include(o => o.Tp_Ocorrencia)
                .Include(o => o.Organization)
                .Include(o => o.Status)
                .FirstOrDefaultAsync();

            if (occurrence == null)
            {
                return NotFound();
            }

            return View(occurrence);
        }

        // GET: Occurrences/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var users = await _context.Users.Where(c => c.OrganizationId == user.OrganizationId).ToListAsync();
            ViewBag.AppraiserId = new SelectList(users, "Id", "Name", user);
            ViewBag.Types = new SelectList(_context.TP_Ocorrencias.OrderByDescending(c => c.CreatedAt), "Id", "Name");
            return View(new CreateOccurrenceModel() { CreatedById = user.Id });
        }

        // POST: Occurrences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Harmed,Document,Cost,Appraiser,Origin,Id,TypeOccurrence,Additional2,Additional1,CreatedById,DataOccurrence,CorretiveAction")] CreateOccurrenceModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
                var appraiser = await _userManager.FindByIdAsync(model.Appraiser.ToString());
                if (user.OrganizationId == null) return NotFound();

                Occurrence occurrence = new Occurrence(
                    model.TypeOccurrence,
                    model.Description,
                    model.Harmed,
                    model.Document,
                    model.Cost,
                    DateTime.Now,
                    DateTime.Now,
                    appraiser.Id,
                    model.Origin,
                    user.Id,
                    user.OrganizationId,
                    4  // em analise                  
                    );
                occurrence.StatusActions = "Não há ação cadastrada ";
                occurrence.Id = model.Id;
                occurrence.Additional1 = model.Additional1;
                occurrence.Additional2 = model.Additional2;
                occurrence.CreatedById = user.Id;
                occurrence.DateOccurrence = model.DataOccurrence;
                occurrence.CorrectiveAction = model.CorretiveAction;
                var typeName = await _context.TP_Ocorrencias.FindAsync(model.TypeOccurrence);
                _context.Occurrences.Add(occurrence);
                string linkOccurrence = Url.Action("Details", "Occurrences", new { idOccurrence = model.Id }, Request.Scheme) ?? "";// $"{Ip}/Occurrences/Details?idOccurrence={occurrence.Id}";//Url.Action("Details", "Occurrences", new { idOccurrence = model.Id }, Request.Scheme) ?? "";
                try
                {
                    await _context.SaveChangesAsync();
                    await _mailService.SendEmailAsync(new EmailRequest()
                    {
                        ToEmail = appraiser.Email,
                        Subject = "Há uma ocorrência aguardando sua avaliação",
                        View = Message(linkOccurrence, "Há uma ocorrência aguardando sua avaliação")
                    });
                }
                catch (Exception e)
                {
                    return RedirectToAction(nameof(Error), new ErrorViewModel { Message = $"error Code 40: {e.Message}" });
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.AppraiserId = new SelectList(_context.Users, "Id", "Name", model.Appraiser);
            ViewBag.Types = new SelectList(_context.TP_Ocorrencias.OrderByDescending(c => c.CreatedAt), "Id", "Name");
            return View(model);
        }

        // GET: Occurrences/Edit/5
        public async Task<IActionResult> Edit(string? idOccurrence)
        {
            if (string.IsNullOrEmpty(idOccurrence))
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences
                .Include(o => o.Appraiser)
                .Include(o => o.Status)
                .Include(o => o.Tp_Ocorrencia)
                .Where(x => x.Id.Equals(idOccurrence))
                .FirstOrDefaultAsync();
            if (occurrence != null)
            {
                bool permitEdit = occurrence.AppraiserId.Equals(User.Identity.GetUserId()) | occurrence.CreatedById.Equals(User.Identity.GetUserId()) | User.IsInRole("Administrador");
                if (!permitEdit)
                {
                    ViewBag.Message = "Você não tem permissão para alterar o registro";
                    return View("Info");
                }
            }
            else
            {
                return View(nameof(Error), new ErrorViewModel { Message = "^Não existe ocorrência com esse ID" });
            }

            EditOccurrenceViewModel model = new EditOccurrenceViewModel(
                occurrence.Id,
                occurrence.TP_OcorrenciaId,
                occurrence.Description,
                occurrence.Harmed,
                occurrence.Document,
                occurrence.Cost,
                occurrence.AppraiserId,
                occurrence.ApplicationUserId,
                occurrence.Origin,
                occurrence.OrganizationId,
                occurrence.Status,
                occurrence.CorrectiveAction
                );
            model.Additional1 = occurrence.Additional1;
            model.Additional2 = occurrence.Additional2;
            model.DataOccurrence = occurrence.DateOccurrence;
            ViewBag.AppraiserId = new SelectList(
                         _context.Users.Where(x => x.OrganizationId == occurrence.OrganizationId),
                         "Id",
                         "Name",
                         occurrence.ApplicationUserId
                         );
            ViewBag.Status = new SelectList(
                         _context.TP_StatusOccurences,
                         "Id",
                         "Name",
                         occurrence.Status
                         );
            ViewBag.Types = new SelectList(
                _context.TP_Ocorrencias.OrderByDescending(c => c.CreatedAt),
                "Id",
                "Name",
                occurrence.TP_OcorrenciaId
                );
            return View(model);
        }

        // POST: Occurrences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TypeOccurrence,Description,Harmed,Document,Cost,AppraiserId,Origin,Id,Appraiser,ApplicationUserId,OrganizationId,CorretiveActions,Status,Status.Name,Additional1,Additional2,DataOccurrence")] EditOccurrenceViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Occurrence occurrence = await _context.Occurrences
                    .Include(o => o.Appraiser)
                    .FirstAsync(c => c.Id.Equals(model.Id));

                occurrence.TP_OcorrenciaId = model.TypeOccurrence;
                occurrence.Description = model.Description;
                occurrence.Harmed = model.Harmed;
                occurrence.Document = model.Document;
                occurrence.Cost = model.Cost;
                occurrence.AppraiserId = model.AppraiserId;
                occurrence.Origin = model.Origin;
                occurrence.Id = model.Id;
                occurrence.ApplicationUserId = model.ApplicationUserId;
                occurrence.OrganizationId = model.OrganizationId;
                occurrence.StatusId = model.Status.Id;
                occurrence.CorrectiveAction = model.CorretiveActions;
                occurrence.Additional1 = model.Additional1;
                occurrence.Additional2 = model.Additional2;
                occurrence.DateOccurrence = model.DataOccurrence;
                string linkOccurrence = Url.Action("Details", "Occurrences", new { idOccurrence = occurrence.Id }, Request.Scheme) ?? ""; //$"{Ip}/Occurrences/Details?idOccurrence={occurrence.Id}";   //Url.Action ("Details", "Occurrences", new { idOccurrence = }, Request.Scheme) ?? ""; 
                try
                {
                    _context.Update(occurrence);
                    await _context.SaveChangesAsync();
                    AlternateView view = Message(linkOccurrence, "Há uma ocorrência aguardando sua avaliação");
                    var toEmail = await _userManager.FindByIdAsync(model.AppraiserId);
                    try
                    {
                        await _mailService.SendEmailAsync(new EmailRequest()
                        {
                            ToEmail = toEmail.Email,
                            Subject = "Há uma ocorrência aguardando sua avaliação",
                            View = view
                        }); ;
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction(nameof(Error), new ErrorViewModel() { Message = "Não foi possível notificar o usuário! \n Erro code 10  Error: " + e.Message });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccurrenceExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AppraiserId = new SelectList(
                         _context.Users.Where(x => x.OrganizationId == model.OrganizationId),
                         "Id",
                         "Name",
                         model.ApplicationUserId
                         );
            ViewBag.Status = new SelectList(
                        _context.TP_StatusOccurences,
                        "Id",
                        "Name",
                        model.Status
                        );
            ViewBag.Types = new SelectList(
                _context.TP_Ocorrencias.OrderByDescending(c => c.CreatedAt),
                "Id",
                "Name",
                model.TypeOccurrence
                );
            return View(model);
        }

        // GET: Occurrences/Delete/5
        public async Task<IActionResult> Delete(string? idOccurrence)
        {
            if (string.IsNullOrEmpty(idOccurrence))
            {
                return View(nameof(Error), new ErrorViewModel() { Message = "Id inválido" });
            }
            var occurrence = await _context.Occurrences
                .Include(o => o.ApplicationUser)
                .Include(o => o.Appraiser)
                .Include(o => o.Organization)
                .Include(o => o.Tp_Ocorrencia)
                .FirstOrDefaultAsync(c => c.Id.Equals(idOccurrence));

            if (occurrence != null)
            {
                bool permitEdit = occurrence.AppraiserId.Equals(User.Identity.GetUserId()) | occurrence.CreatedById.Equals(User.Identity.GetUserId()) | User.IsInRole("Administrador");
                if (!permitEdit)
                {
                    ViewBag.Message = "Você não tem permissão para alterar o registro";
                    return View("Info");
                }
                else
                {
                    return View(occurrence);
                }
            }
            else
            {
                return View(nameof(Error), new ErrorViewModel() { Message = "Não existe ocorrência com esse ID" });
            }
        }

        // POST: Occurrences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("Id")] Occurrence occurrence)
        {
            //var deleteOccurrence = await _context.Occurrences.FindAsync(occurrence.Id);
            try
            {
                _context.Occurrences.Remove(occurrence);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View(nameof(Error), new ErrorViewModel() { Message = $"Não foi possível deletar essa ocorrência \n {e.Message}" });
            }

        }

        private bool OccurrenceExists(string id)
        {
            return _context.Occurrences.Any(e => e.Id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
        }
        private AlternateView Message(string link, string title)
        {
            // <img src='{path}/css/Images/header.png' alt='imagem email' />
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

        public async Task<JsonResult> GetTypesOccurrencesPerOrganization(string organizationId)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var result = await _context.TP_Ocorrencias
                .Where(c => c.OrganizationId.Equals(user.OrganizationId))
                .Select(c => new TP_Ocorrencia()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
            return Json(result);
        }

        public async Task<IActionResult> GetStatusOcurrence(string organization)
        {
            var result = await _context.TP_StatusOccurences.ToListAsync();
            return Json(result);
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateColumn(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Json("Titulo não pode ser vazio");
            }
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            await _service.AddColumn(user.OrganizationId ?? "", title);

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInfoComplement(string occurrenceId, string value, int columnId)
        {
            if (string.IsNullOrEmpty(occurrenceId) || string.IsNullOrEmpty(value) || columnId == 0)
            {
                return Json("Não é possivel adicionar a informação complementar");
            }
            await _service.AddOccurrenceValueComplement(columnId, occurrenceId, value);
            return RedirectToAction("Index");
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComplement()//int id,string value, string occurrenceId
        {
            string[] values = HttpContext.Request.Form["value"];
            string[] ids = HttpContext.Request.Form["id"];
            string[] occurrenceId = HttpContext.Request.Form["occurrenceId"];

            if (ids.Length > 0)
            {
                bool[] result = await _service.EditValueComplement(ids, values, occurrenceId[1]);
                if (result[1])
                {
                    return RedirectToAction(nameof(Edit), new { idOccurrence = occurrenceId[1] });
                }
                ModelState.AddModelError(String.Empty, "Não foi possivel atualizar!");
            }
            return RedirectToAction(nameof(Edit), new { idOccurrence = occurrenceId[1] });
        }
    }
}

