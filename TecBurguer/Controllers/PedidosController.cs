
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetPedidos(int id)
    {
        if (_context.Pedidos == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .Include(p => p.IdUsuarioNavigation)
            .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedido == null)
        {
            return NotFound();
        }

        return pedido;
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> PutPedido(int id, Pedido pedido)
    {
        if (id != pedido.IdPedido)
        {
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

    // ... (Seu método PutPedido termina aqui)

    // PATCH: api/Pedidos/update/5
    [HttpPatch("update/{id}")]
    public async Task<IActionResult> PatchPedido(int id, [FromBody] Pedido dadosParciais)
    {
        // 1. Encontrar o pedido existente no banco
        var pedidoDoBanco = await _context.Pedidos.FindAsync(id);
        if (pedidoDoBanco == null)
        {
            return NotFound();
        }

        // 2. Aplicar manualmente as atualizações parciais
        //    Isso garante que só mudamos o que veio na requisição
        //    e não apagamos os outros campos para null.

        if (dadosParciais.Nome != null)
        {
            pedidoDoBanco.Nome = dadosParciais.Nome;
        }
        if (dadosParciais.Descricao != null)
        {
            pedidoDoBanco.Descricao = dadosParciais.Descricao;
        }
        if (dadosParciais.PrecoTotal != null)
        {
            pedidoDoBanco.PrecoTotal = dadosParciais.PrecoTotal;
        }
        if (dadosParciais.Estado != null)
        {
            pedidoDoBanco.Estado = dadosParciais.Estado;
        }
        if (dadosParciais.IdUsuario != null)
        {
            pedidoDoBanco.IdUsuario = dadosParciais.IdUsuario;
        }

        // 3. Marcar a entidade como modificada e salvar
        _context.Entry(pedidoDoBanco).State = EntityState.Modified;

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
