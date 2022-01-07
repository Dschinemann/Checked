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
using Checked.Models.ViewModels;

namespace Checked.Controllers
{
    public class ActionsController : Controller
    {
        private readonly CheckedDbContext _context;

        public ActionsController(CheckedDbContext context)
        {
            _context = context;
        }

        // GET: Actions
        public async Task<IActionResult> Index(int id)
        {
            var result = await _context.Actions
                .Where(x => x.OccurrenceId == id)
                .FirstOrDefaultAsync();
            if(result == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(result);
        }

        // GET: Actions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var action = await _context.Actions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (action == null)
            {
                return NotFound();
            }

            return View(action);
        }

        // GET: Actions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subject,Accountable,Init,Finish,What,Why,Where,Who,When,How,HowMuch")] CreateActionModel model)
        {
            Models.Models.Action action = new Models.Models.Action();
            if (ModelState.IsValid)     
            {
                action.Subject = model.Subject;
                action.Accountable = model.Accountable;
                action.Init = model.Init;
                action.Finish = model.Finish;
                action.What = model.What;
                action.Why = model.Why;
                action.Where = model.Where;
                action.Who = model.Who;
                action.When = model.When;
                action.How = model.How;
                action.HowMuch = model.HowMuch;
                _context.Add(action);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Actions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var action = await _context.Actions.FindAsync(id);
            if (action == null)
            {
                return NotFound();
            }
            return View(action);
        }

        // POST: Actions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Subject,Accountable,Init,Finish,CreatedAt,UpdatedAt,What,Why,Where,Who,When,How,HowMuch,TP_Status")] Models.Models.Action action)
        {
            if (id != action.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(action);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionExists(action.Id))
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
            return View(action);
        }

        // GET: Actions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var action = await _context.Actions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (action == null)
            {
                return NotFound();
            }

            return View(action);
        }

        // POST: Actions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var action = await _context.Actions.FindAsync(id);
            _context.Actions.Remove(action);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActionExists(int id)
        {
            return _context.Actions.Any(e => e.Id == id);
        }
    }
}
