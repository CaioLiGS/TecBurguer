using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TecBurguer.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetPedidos(int id)
    {
        if (_context.Pedidos == null)
        {
            return NotFound();
        }
        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
        {
            return NotFound();
        }

        return pedido;
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> PutPedido(int id, Pedido pedido)
    {

        Console.WriteLine(id);
        Console.WriteLine(pedido.IdPedido);

        if (id != pedido.IdPedido)
        {
            Console.WriteLine("Bad Request");
            return BadRequest();
        }

        _context.Entry(pedido).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PedidoExists(id))
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

    private bool PedidoExists(int id)
    {
        return (_context.Pedidos?.Any(e => e.IdPedido == id)).GetValueOrDefault();
    }
}