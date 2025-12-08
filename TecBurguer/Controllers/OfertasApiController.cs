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
    public class OfertasApiController : ControllerBase
    {
        private readonly DBTecBurguerContext _context;

        public OfertasApiController(DBTecBurguerContext context)
        {
            _context = context;
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Ofertas>>> GetOfertas()
        {

            if (_context.Ofertas == null)
            {
                return NotFound();
            }

            var ofertasVencidas = await _context.Ofertas
                .Where(o => o.dataTermino < DateTime.Now)
                .ToListAsync();

            if (ofertasVencidas.Any())
            {
                _context.Ofertas.RemoveRange(ofertasVencidas);
                await _context.SaveChangesAsync();
            }

           
            return await _context.Ofertas.ToListAsync();
        }
    }
}
