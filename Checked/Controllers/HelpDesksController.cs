#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Checked.Data;
using Checked.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Checked.Models;
using System.Diagnostics;

namespace Checked.Controllers
{
    public class HelpDesksController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        public HelpDesksController(CheckedDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: HelpDesks
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            return View(await _context.HelpDesks
                .Where(c => c.UserId.Equals(user.Id))
                .ToListAsync()
                );
        }

        // GET: HelpDesks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helpDesk = await _context.HelpDesks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (helpDesk == null)
            {
                return NotFound();
            }

            return View(helpDesk);
        }

        // GET: HelpDesks/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            HelpDesk model = new HelpDesk()
            {
                User = user,
                UserId = user.Id                
            };
            return View(model);
        }

        // POST: HelpDesks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UrlAccess,ErrorMessage,Message,UserId, User")] HelpDesk helpDesk)
        {
            if (ModelState.IsValid)
            {
                _context.HelpDesks.Add(helpDesk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(helpDesk);
        }

        // GET: HelpDesks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helpDesk = await _context.HelpDesks.FindAsync(id);
            if (helpDesk == null)
            {
                return NotFound();
            }
            return View(helpDesk);
        }

        // POST: HelpDesks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UrlAccess,ErrorMessage,Message")] HelpDesk helpDesk)
        {
            if (id != helpDesk.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(helpDesk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e )
                {
                    if (!HelpDeskExists(helpDesk.Id))
                    {
                        return View(nameof(Error), new { Message = $"Id não encontrado" });
                    }
                    else
                    {
                        return View(nameof(Error), new { Message = $"Não foi possivel atualizar os registros - errorCode 01 --  {e.Message}" });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(helpDesk);
        }

        // GET: HelpDesks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helpDesk = await _context.HelpDesks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (helpDesk == null)
            {
                return NotFound();
            }

            return View(helpDesk);
        }

        // POST: HelpDesks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var helpDesk = await _context.HelpDesks.FindAsync(id);
            _context.HelpDesks.Remove(helpDesk);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HelpDeskExists(int id)
        {
            return _context.HelpDesks.Any(e => e.Id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
        }
    }
}
