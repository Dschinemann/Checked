using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using Checked.Data;
using Checked.Models.Models;
using Checked.Models.ViewModels;

namespace Checked.Controllers
{
    public class ActionsController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        public ActionsController(CheckedDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Actions
        public async Task<IActionResult> Index()
        {
            var checkedDbContext = _context.Actions.Include(a => a.Plan);
            return View(await checkedDbContext.ToListAsync());
        }

        // GET: Actions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var action = await _context.Actions
                .Include(a => a.Plan)
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
            CreateActionViewModel model = new CreateActionViewModel();
            model.PlanId = id;
            return View(model);
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("What,Why,Where,Who,Init,Finish,How,HowMuch, PlanId")] CreateActionViewModel model, int id)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var plan = await _context.Plans.Where(c => c.Id == id).FirstOrDefaultAsync();
            if(plan.organizationId != user.OrganizationId)
            {
                ModelState.AddModelError(string.Empty,"Id da Empresa difere do Id do Usuário");
            }

            if (id == 0) return NotFound();
            if (ModelState.IsValid)
            {
                Models.Models.Action action = new Models.Models.Action();
                action.CreatedAt = DateTime.Now;
                action.UpdatedAt = DateTime.Now;
                action.How = model.How;
                action.Where = model.Where;
                action.PlanId = id;
                action.Init = model.Init;
                action.Finish = model.Finish;
                action.NewFinish = model.Finish;
                action.HowMuch = model.HowMuch;
                action.What = model.What;
                action.Who = model.Who;
                action.Why = model.Why;
                action.TP_Status = Models.Enums.TP_Status.Aberto;


                _context.Add(action);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Plans",new { planId = model.PlanId});
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
                Who = action.Who,
                Why = action.Why
            };            
            return View(model);
        }

        // POST: Actions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,What,Why,Where,Who,Init,Finish,NewFinish,How,HowMuch,PlanId,status")] CreateActionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Models.Models.Action action = new Models.Models.Action()
                    {
                        Id = (int)model.Id,
                        What = model.What,
                        Why =model.Why,
                        Where = model.Where,
                        Who = model.Who,
                        Init = model.Init,
                        Finish = model.Finish,
                        NewFinish = model.NewFinish,
                        How = model.How,
                        HowMuch = model.HowMuch,
                        UpdatedAt = DateTime.Now,
                        PlanId = model.PlanId,
                        TP_Status = model.status
                    };
                    _context.Update(action);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionExists((int)model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Plans", new { planId = model.PlanId});
            }            
            return View(model);
        }

        // GET: Actions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var action = await _context.Actions
                .Include(a => a.Plan)
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
            return RedirectToAction("Index", "Plans", new { planId = action.PlanId });
        }

        private bool ActionExists(int id)
        {
            return _context.Actions.Any(e => e.Id == id);
        }
    }
}
