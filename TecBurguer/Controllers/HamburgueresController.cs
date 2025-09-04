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
    public class HamburgueresController : Controller
    {
        private readonly DBTecBurguerContext _context;

        public HamburgueresController(DBTecBurguerContext context)
        {
            _context = context;
        }

        // GET: Hamburgueres
        public async Task<IActionResult> Index()
        {
              return _context.Hamburguers != null ? 
                          View(await _context.Hamburguers.ToListAsync()) :
                          Problem("Entity set 'DBTecBurguerContext.Hamburguers'  is null.");
        }

        // GET: Hamburgueres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hamburguers == null)
            {
                return NotFound();
            }

            var hamburguer = await _context.Hamburguers
                .FirstOrDefaultAsync(m => m.IdHamburguer == id);
            if (hamburguer == null)
            {
                return NotFound();
            }

            return View(hamburguer);
        }

        // GET: Hamburgueres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hamburgueres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHamburguer,Nome,Descricao,Preco")] Hamburguer hamburguer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hamburguer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hamburguer);
        }

        // GET: Hamburgueres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Hamburguers == null)
            {
                return NotFound();
            }

            var hamburguer = await _context.Hamburguers.FindAsync(id);
            if (hamburguer == null)
            {
                return NotFound();
            }
            return View(hamburguer);
        }

        // POST: Hamburgueres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHamburguer,Nome,Descricao,Preco")] Hamburguer hamburguer)
        {
            if (id != hamburguer.IdHamburguer)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hamburguer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HamburguerExists(hamburguer.IdHamburguer))
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
            return View(hamburguer);
        }

        // GET: Hamburgueres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Hamburguers == null)
            {
                return NotFound();
            }

            var hamburguer = await _context.Hamburguers
                .FirstOrDefaultAsync(m => m.IdHamburguer == id);
            if (hamburguer == null)
            {
                return NotFound();
            }

            return View(hamburguer);
        }

        // POST: Hamburgueres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Hamburguers == null)
            {
                return Problem("Entity set 'DBTecBurguerContext.Hamburguers'  is null.");
            }
            var hamburguer = await _context.Hamburguers.FindAsync(id);
            if (hamburguer != null)
            {
                _context.Hamburguers.Remove(hamburguer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HamburguerExists(int id)
        {
          return (_context.Hamburguers?.Any(e => e.IdHamburguer == id)).GetValueOrDefault();
        }
    }
}
