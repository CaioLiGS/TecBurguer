
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TecBurguer.Models;

[ApiController]
[Route("api/[controller]")]
public class PedidoHamburgueresController : ControllerBase
{
    private readonly DBTecBurguerContext _context;

    public PedidoHamburgueresController(DBTecBurguerContext context)
    {
        _context = context;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] PedidoHamburguer pedido)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.PedidoHamburgueres.Add(pedido);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Create), new { id = pedido.Id }, pedido);
    }

    [HttpGet("Listar")]
    public async Task<ActionResult<IEnumerable<PedidoHamburguer>>> GetPedidoHamburgueres()
    {
        if (_context.PedidoHamburgueres == null)
        {
            return NotFound();
        }

        var PedidoHamburgueres = _context.PedidoHamburgueres
            .Include(h => h.IdHamburguerNavigation);

        return await PedidoHamburgueres.ToListAsync();
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> PutPedidoHamburgueres(int id, PedidoHamburguer pedidoHamburguer)
    {

        if (id != pedidoHamburguer.Id)
        {
            return BadRequest();
        }

        _context.Entry(pedidoHamburguer).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PedidoHamburguerExists(id))
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

    private bool PedidoHamburguerExists(int id)
    {
        return (_context.PedidoHamburgueres?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePedidoHamburguer(int id)
    {
        var pedidoHamburguer = await _context.PedidoHamburgueres.FindAsync(id);
    
        if (pedidoHamburguer == null)
        {
            return NotFound();
        }
    
        _context.PedidoHamburgueres.Remove(pedidoHamburguer);
        await _context.SaveChangesAsync();
    
        return NoContent();
    }

}
