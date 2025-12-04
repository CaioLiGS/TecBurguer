using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    public class BebidasController : Controller
    {
        private readonly DBTecBurguerContext _context;

        public BebidasController(DBTecBurguerContext context)
        {
            _context = context;
        }

        // GET: Bebidas
        public async Task<IActionResult> Index()
        {
              return _context.Bebidas != null ? 
                          View(await _context.Bebidas.ToListAsync()) :
                          Problem("Entity set 'DBTecBurguerContext.Bebidas'  is null.");
        }

        // GET: Bebidas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bebidas == null)
            {
                return NotFound();
            }

            var bebida = await _context.Bebidas
                .FirstOrDefaultAsync(m => m.IdBebidas == id);
            if (bebida == null)
            {
                return NotFound();
            }

            return View(bebida);
        }

        // GET: Bebidas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bebidas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBebida,Nome,Preco,Imagem,Quantidade")] Bebidas bebida)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bebida);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bebida);
        }

        // GET: Bebidas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bebidas == null)
            {
                return NotFound();
            }

            var bebida = await _context.Bebidas.FindAsync(id);
            if (bebida == null)
            {
                return NotFound();
            }
            return View(bebida);
        }

        // POST: Bebidas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBebida,Nome,Preco,Imagem,Quantidade")] Bebidas bebida)
        {
            if (id != bebida.IdBebidas)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bebida);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BebidaExists(bebida.IdBebidas))
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
            return View(bebida);
        }

        // GET: Bebidas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bebidas == null)
            {
                return NotFound();
            }

            var bebida = await _context.Bebidas
                .FirstOrDefaultAsync(m => m.IdBebidas == id);
            if (bebida == null)
            {
                return NotFound();
            }

            return View(bebida);
        }

        // POST: Bebidas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bebidas == null)
            {
                return Problem("Entity set 'DBTecBurguerContext.Bebidas'  is null.");
            }
            var bebida = await _context.Bebidas.FindAsync(id);
            if (bebida != null)
            {
                _context.Bebidas.Remove(bebida);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BebidaExists(int id)
        {
          return (_context.Bebidas?.Any(e => e.IdBebidas == id)).GetValueOrDefault();
        }
    }
}
