using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Checked.Data;
using Checked.Models.Models;

namespace Checked.Controllers
{
    public class OccurrencesController : Controller
    {
        private readonly CheckedDbContext _context;

        public OccurrencesController(CheckedDbContext context)
        {
            _context = context;
        }

        // GET: Occurrences
        public async Task<IActionResult> Index()
        {
            var checkedDbContext = _context.Occurrences.Include(o => o.ApplicationUser).Include(o => o.Appraiser).Include(o => o.Organization);
            return View(await checkedDbContext.ToListAsync());
        }

        // GET: Occurrences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences
                .Include(o => o.ApplicationUser)
                .Include(o => o.Appraiser)
                .Include(o => o.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (occurrence == null)
            {
                return NotFound();
            }

            return View(occurrence);
        }

        // GET: Occurrences/Create
        public IActionResult Create()
        {
            ViewData["AppraiserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Occurrences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Harmed,Document,Cost,AppraiserId,Origin")] Occurrence occurrence)
        {
            if (ModelState.IsValid)
            {
                _context.Add(occurrence);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppraiserId"] = new SelectList(_context.Users, "Id", "Name", occurrence.AppraiserId);
            return View(occurrence);
        }

        // GET: Occurrences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences.FindAsync(id);
            if (occurrence == null)
            {
                return NotFound();
            }
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", occurrence.ApplicationUserId);
            ViewData["AppraiserId"] = new SelectList(_context.Users, "Id", "Name", occurrence.AppraiserId);
            //ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", occurrence.OrganizationId);
            return View(occurrence);
        }

        // POST: Occurrences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Harmed,Document,Cost,AppraiserId,Origin")] Occurrence occurrence)
        {
            if (id != occurrence.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(occurrence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccurrenceExists(occurrence.Id))
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
            ViewData["AppraiserId"] = new SelectList(_context.Users, "Id", "Name", occurrence.AppraiserId);
            return View(occurrence);
        }

        // GET: Occurrences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var occurrence = await _context.Occurrences
                .Include(o => o.ApplicationUser)
                .Include(o => o.Appraiser)
                .Include(o => o.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (occurrence == null)
            {
                return NotFound();
            }

            return View(occurrence);
        }

        // POST: Occurrences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var occurrence = await _context.Occurrences.FindAsync(id);
            _context.Occurrences.Remove(occurrence);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OccurrenceExists(int id)
        {
            return _context.Occurrences.Any(e => e.Id == id);
        }
    }
}
