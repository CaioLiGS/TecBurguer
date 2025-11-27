using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientesApiController : ControllerBase
    {
        private readonly DBTecBurguerContext _context;

        public IngredientesApiController(DBTecBurguerContext context)
        {
            _context = context;
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Ingrediente>>> GetIngredientes()
        {
            if (_context.Ingredientes == null)
            {
                return NotFound();
            }
            return await _context.Ingredientes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredienteApi(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            return Ok(ingrediente);
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> PatchIngrediente(int id, [FromBody] Ingrediente dados)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente== null)
            {
                return NotFound();
            }

            if (dados.Nome != null)
            {
                ingrediente.Nome = dados.Nome;
            }
            if (dados.Quantidade != null)
            {
                ingrediente.Quantidade = dados.Quantidade;
            }
            
            _context.Entry(ingrediente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool IngredienteExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
