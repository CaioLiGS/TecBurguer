using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HamIngApi : ControllerBase
    {
        private readonly DBTecBurguerContext _context;

        public HamIngApi(DBTecBurguerContext context)
        {
            _context = context;
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<HamburguerIgrediente>>> GetHamburguerIngredientes()
        {
            if (_context.HamburguerIgredientes == null)
            {
                return NotFound();
            }
            return await _context.HamburguerIgredientes.ToListAsync();
        }
    }
}
