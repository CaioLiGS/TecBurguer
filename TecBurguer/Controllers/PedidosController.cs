using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TecBurguer.Models;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly DBTecBurguerContext _context;

    public PedidosController(DBTecBurguerContext context)
    {
        _context = context;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] Pedido pedido)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        _context.Pedidos.Add(pedido);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Create), new { id = pedido.IdPedido }, pedido);
    }

    [HttpGet("Listar")]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
    {
        if (_context.Pedidos == null)
        {
            return NotFound();
        }
        return await _context.Pedidos.ToListAsync();
    }
}