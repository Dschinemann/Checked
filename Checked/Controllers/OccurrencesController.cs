using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Checked.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Checked.Models;
using System.Diagnostics;

namespace Checked.Controllers
{
    public class OccurrencesController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public OccurrencesController(CheckedDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Occurrences
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.OrganizationId == null)
            {
                return RedirectToAction("Create", "Organizations");
            }
            var occurrences = await _context.Occurrences
                .Where(c => c.OrganizationId == user.OrganizationId)
                .Include(o => o.ApplicationUser)
                .Include(o => o.Tp_Ocorrencia)
                .Include(o => o.Status)
                .ToListAsync();

            return View(occurrences);
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
            ViewBag.Types = new SelectList(_context.TP_Ocorrencias, "Id", "Name");
            return View(new CreateOccurrenceModel() { CreatedById = user.Id});
        }

        // POST: Occurrences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Harmed,Document,Cost,Appraiser,Origin,Id,TypeOccurrence,Additional2,Additional1,CreatedById")] CreateOccurrenceModel model)
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
                var typeName = await _context.TP_Ocorrencias.FindAsync(model.TypeOccurrence);                
                _context.Occurrences.Add(occurrence);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AppraiserId = new SelectList(_context.Users, "Id", "Name", model.Appraiser);
            ViewBag.Types = new SelectList(_context.TP_Ocorrencias, "Id", "Name");
            return View(model);
        }

        // GET: Occurrences/Edit/5
        public async Task<IActionResult> Edit(string? idOccurrence)
        {
            if (idOccurrence.Equals("") || idOccurrence == null)
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences
                .Include(o => o.Appraiser)
                .Include(o => o.Status)
                .Include(o => o.Tp_Ocorrencia)
                .Where(x => x.Id.Equals(idOccurrence))
                .FirstOrDefaultAsync();
            if (occurrence == null)
            {
                return NotFound();
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
                _context.TP_Ocorrencias,
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
        public async Task<IActionResult> Edit(string id, [Bind("TypeOccurrence,Description,Harmed,Document,Cost,AppraiserId,Origin,Id,Appraiser,ApplicationUserId,OrganizationId,CorretiveActions,Status,Status.Name,Additional1,Additional2")] EditOccurrenceViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Occurrence occurrence = await _context.Occurrences.FindAsync(id);

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
                
                try
                {
                    _context.Update(occurrence);
                    await _context.SaveChangesAsync();
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
                _context.TP_Ocorrencias,
                "Id",
                "Name",
                model.TypeOccurrence
                );
            return View(model);
        }

        // GET: Occurrences/Delete/5
        public async Task<IActionResult> Delete(string? idOccurrence)
        {
            if (idOccurrence == null || idOccurrence.Equals(""))
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences
                .Include(o => o.ApplicationUser)
                .Include(o => o.Appraiser)
                .Include(o => o.Organization)
                .Include(o => o.Tp_Ocorrencia)
                .FirstOrDefaultAsync(c => c.Id.Equals(idOccurrence));

            if (occurrence == null)
            {
                return NotFound();
            }

            return View(occurrence);
        }

        // POST: Occurrences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Occurrence occurrence)
        {
            //var occurrence = await _context.Occurrences.FindAsync(idOccurrence);
            _context.Occurrences.Remove(occurrence);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
    }
}
