using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HamburguerIgredientesController : Controller
    {
        private readonly DBTecBurguerContext _context;

        public HamburguerIgredientesController(DBTecBurguerContext context)
        {
            _context = context;
        }

        // GET: HamburguerIgredientes
        public async Task<IActionResult> Index()
        {
            var dBTecBurguerContext = _context.HamburguerIgredientes.Include(h => h.IdHamburguerNavigation).Include(h => h.IdIngredienteNavigation);
            return View(await dBTecBurguerContext.ToListAsync());
        }

        // GET: HamburguerIgredientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HamburguerIgredientes == null)
            {
                return NotFound();
            }

            var hamburguerIgrediente = await _context.HamburguerIgredientes
                .Include(h => h.IdHamburguerNavigation)
                .Include(h => h.IdIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hamburguerIgrediente == null)
            {
                return NotFound();
            }

            return View(hamburguerIgrediente);
        }

        // GET: HamburguerIgredientes/Create
        public IActionResult Create()
        {
            ViewData["IdHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "IdHamburguer");
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente");
            return View();
        }

        // POST: HamburguerIgredientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdHamburguer,IdIngrediente,QuantidadeNecessario")] HamburguerIgrediente hamburguerIgrediente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hamburguerIgrediente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "IdHamburguer", hamburguerIgrediente.IdHamburguer);
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente", hamburguerIgrediente.IdIngrediente);
            return View(hamburguerIgrediente);
        }

        // GET: HamburguerIgredientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HamburguerIgredientes == null)
            {
                return NotFound();
            }

            var hamburguerIgrediente = await _context.HamburguerIgredientes.FindAsync(id);
            if (hamburguerIgrediente == null)
            {
                return NotFound();
            }
            ViewData["IdHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "IdHamburguer", hamburguerIgrediente.IdHamburguer);
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente", hamburguerIgrediente.IdIngrediente);
            return View(hamburguerIgrediente);
        }

        // POST: HamburguerIgredientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdHamburguer,IdIngrediente,QuantidadeNecessario")] HamburguerIgrediente hamburguerIgrediente)
        {
            if (id != hamburguerIgrediente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hamburguerIgrediente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HamburguerIgredienteExists(hamburguerIgrediente.Id))
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
            ViewData["IdHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "IdHamburguer", hamburguerIgrediente.IdHamburguer);
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente", hamburguerIgrediente.IdIngrediente);
            return View(hamburguerIgrediente);
        }

        // GET: HamburguerIgredientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HamburguerIgredientes == null)
            {
                return NotFound();
            }

            var hamburguerIgrediente = await _context.HamburguerIgredientes
                .Include(h => h.IdHamburguerNavigation)
                .Include(h => h.IdIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hamburguerIgrediente == null)
            {
                return NotFound();
            }

            return View(hamburguerIgrediente);
        }

        // POST: HamburguerIgredientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HamburguerIgredientes == null)
            {
                return Problem("Entity set 'DBTecBurguerContext.HamburguerIgredientes'  is null.");
            }
            var hamburguerIgrediente = await _context.HamburguerIgredientes.FindAsync(id);
            if (hamburguerIgrediente != null)
            {
                _context.HamburguerIgredientes.Remove(hamburguerIgrediente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HamburguerIgredienteExists(int id)
        {
          return (_context.HamburguerIgredientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
