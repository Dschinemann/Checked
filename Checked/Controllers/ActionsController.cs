using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Checked.Servicos.ControllerServices;

namespace Checked.Controllers
{
    public class ActionsController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly ActionsService _actionsService;

        public ActionsController(CheckedDbContext context,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            ActionsService service)
        {
            _context = context;
            _userManager = userManager;
            _actionsService = service;
        }

        // GET: Actions
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var result = await _actionsService.ListActionAsync(id);
            IndexModelAction viewModel = new IndexModelAction();
            viewModel.Actions = result;        

            if (result == null)
            {
                return RedirectToAction(nameof(Create), new { id = id });
            }
            return View(viewModel);
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
        public IActionResult Create(int id)
        {
            CreateActionModel model = new CreateActionModel();
            model.OccurrenceId = id;
            return View(model);
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subject,Accountable,Init,Finish,What,Why,Where,Who,When,How,HowMuch")] CreateActionModel model, int id)
        {
            Models.Models.Action action = new Models.Models.Action();
            ApplicationUser user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
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
                action.OrganizationId = user.OrganizationId ?? 0;
                action.ApplicationUserId = user.Id;
                action.OccurrenceId = id;
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
