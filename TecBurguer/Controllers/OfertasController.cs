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
    public class OfertasController : Controller
    {
        private readonly DBTecBurguerContext _context;

        public OfertasController(DBTecBurguerContext context)
        {
            _context = context;
        }

        // GET: Ofertas
        public async Task<IActionResult> Index()
        {
            var dBTecBurguerContext = _context.Ofertas.Include(o => o.idHamburguerNavigation);
            return View(await dBTecBurguerContext.ToListAsync());
        }

        // GET: Ofertas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ofertas == null)
            {
                return NotFound();
            }

            var ofertas = await _context.Ofertas
                .Include(o => o.idHamburguerNavigation)
                .FirstOrDefaultAsync(m => m.id == id);
            if (ofertas == null)
            {
                return NotFound();
            }

            return View(ofertas);
        }

        // POST: Ofertas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public IActionResult Create(int? hamburguerId)
        {
            var oferta = new Ofertas();

            if (hamburguerId.HasValue)
            {
                oferta.idHamburguer = hamburguerId.Value;
            }

            ViewData["idHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "Nome", oferta.idHamburguer);

            var precos = _context.Hamburguers.ToDictionary(h => h.IdHamburguer, h => h.Preco);

            ViewData["PrecosHamburgueres"] = System.Text.Json.JsonSerializer.Serialize(precos);

            return View(oferta);
        }


        //
        // MÉTODO POST: Para RECEBER os dados do formulário
        //
        // **** ESTE MÉTODO PRECISA TER O [HttpPost] ****
        //
        // POST: Ofertas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,idHamburguer,dataTermino,precoFinal,porcentagem")] Ofertas ofertas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ofertas);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Hamburgueres");
            }

            // Se o modelo falhar, recarrega o dropdown e retorna para a View
            ViewData["idHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "Nome", ofertas.idHamburguer);
            return View(ofertas);
        }

        // GET: Ofertas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ofertas == null)
            {
                return NotFound();
            }

            var ofertas = await _context.Ofertas.FindAsync(id);
            if (ofertas == null)
            {
                return NotFound();
            }
            ViewData["idHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "IdHamburguer", ofertas.idHamburguer);
            return View(ofertas);
        }

        // POST: Ofertas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,idHamburguer,dataTermino,precoFinal,porcentagem")] Ofertas ofertas)
        {
            if (id != ofertas.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ofertas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfertasExists(ofertas.id))
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
            ViewData["idHamburguer"] = new SelectList(_context.Hamburguers, "IdHamburguer", "IdHamburguer", ofertas.idHamburguer);
            return View(ofertas);
        }

        // GET: Ofertas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ofertas == null)
            {
                return NotFound();
            }

            var ofertas = await _context.Ofertas
                .Include(o => o.idHamburguerNavigation)
                .FirstOrDefaultAsync(m => m.id == id);
            if (ofertas == null)
            {
                return NotFound();
            }

            return View(ofertas);
        }

        // POST: Ofertas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ofertas == null)
            {
                return Problem("Entity set 'DBTecBurguerContext.Ofertas'  is null.");
            }
            var ofertas = await _context.Ofertas.FindAsync(id);
            if (ofertas != null)
            {
                _context.Ofertas.Remove(ofertas);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfertasExists(int id)
        {
          return (_context.Ofertas?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
