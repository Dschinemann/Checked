using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checked.Data;
using Checked.Models.Types;
using Microsoft.AspNet.Identity;
using Checked.Models.Models;
using Microsoft.AspNetCore.Identity;

namespace Checked.Controllers
{
    public class TP_OcorrenciaController : Controller
    {
        private readonly CheckedDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        public TP_OcorrenciaController(CheckedDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TP_Ocorrencia
        public async Task<IActionResult> Index()
        {
            return View(await _context.TP_Ocorrencias.ToListAsync());
        }

        // GET: TP_Ocorrencia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tP_Ocorrencia = await _context.TP_Ocorrencias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tP_Ocorrencia == null)
            {
                return NotFound();
            }

            return View(tP_Ocorrencia);
        }

        // GET: TP_Ocorrencia/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            
            TP_Ocorrencia model = new TP_Ocorrencia()
            {
                OrganizationId = user.OrganizationId,    
                CreatedById = user.Id
            };
            
            return View(model);
        }

        // POST: TP_Ocorrencia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,OrganizationId,Organization,CreatedById")] TP_Ocorrencia model)
        {
            
            if (ModelState.IsValid)
            {
                if(await TP_OcorrenciaExists(0, model.Name))
                {
                    ModelState.AddModelError(string.Empty, "Já existe um Tipo com esse nome");
                    return View(model);
                }
                _context.TP_Ocorrencias.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create","Occurrences");
            }
            return View(model);
        }

        // GET: TP_Ocorrencia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tP_Ocorrencia = await _context.TP_Ocorrencias.FindAsync(id);
            if (tP_Ocorrencia == null)
            {
                return NotFound();
            }
            return View(tP_Ocorrencia);
        }

        // POST: TP_Ocorrencia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TP_Ocorrencia tP_Ocorrencia)
        {
            if (id != tP_Ocorrencia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tP_Ocorrencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await TP_OcorrenciaExists(tP_Ocorrencia.Id, String.Empty))
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
            return View(tP_Ocorrencia);
        }

        // GET: TP_Ocorrencia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tP_Ocorrencia = await _context.TP_Ocorrencias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tP_Ocorrencia == null)
            {
                return NotFound();
            }

            return View(tP_Ocorrencia);
        }

        // POST: TP_Ocorrencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tP_Ocorrencia = await _context.TP_Ocorrencias.FindAsync(id);
            _context.TP_Ocorrencias.Remove(tP_Ocorrencia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TP_OcorrenciaExists(int id, string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await _context.TP_Ocorrencias.AnyAsync(e => e.Id == id);
            }
            else
            {
                return await _context.TP_Ocorrencias.AnyAsync(e => e.Name.Equals(name));
            }
            
        }
        
    }
}
