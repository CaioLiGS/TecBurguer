
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TecBurguer.Models;

[ApiController]
[Route("api/[controller]")]
public class PedidoBebidasController : ControllerBase
{
    private readonly DBTecBurguerContext _context;

    public PedidoBebidasController(DBTecBurguerContext context)
    {
        _context = context;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] PedidoBebidas pedido)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.PedidoBebidas.Add(pedido);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Create), new { id = pedido.Id }, pedido);
    }

    [HttpGet("Listar")]
    public async Task<ActionResult<IEnumerable<PedidoBebidas>>> GetPedidoBebidas()
    {
        if (_context.PedidoBebidas == null)
        {
            return NotFound();
        }

        return await _context.PedidoBebidas.ToListAsync();
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdatePedidoBebidas(int id, [FromBody] PedidoBebidas pedidoBebidas)
    {
        if (id != pedidoBebidas.Id) return BadRequest();

        var itemExistente = await _context.PedidoBebidas.FindAsync(id);
        if (itemExistente == null) return NotFound();

        var Bebidas = await _context.Bebidas
            .FirstOrDefaultAsync(h => h.IdBebidas == pedidoBebidas.IdBebidas);

        if (pedidoBebidas.Quantidade > itemExistente.Quantidade)
        {
            if (pedidoBebidas.Quantidade > Bebidas?.Quantidade )
            {
                return BadRequest("Estoque insuficiente para a bebida solicitada.");
            }
        }

        _context.Entry(itemExistente).State = EntityState.Detached;
        _context.Entry(pedidoBebidas).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PedidoBebidasExists(id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    private bool PedidoBebidasExists(int id)
    {
        return (_context.PedidoBebidas?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePedidoBebidas(int id)
    {
        var pedidoBebidas = await _context.PedidoBebidas.FindAsync(id);
    
        if (pedidoBebidas == null)
        {
            return NotFound();
        }
    
        _context.PedidoBebidas.Remove(pedidoBebidas);
        await _context.SaveChangesAsync();
    
        return NoContent();
    }
    [HttpGet("ListarPorPedido/{idPedido}")]
    public async Task<ActionResult<IEnumerable<PedidoBebidasDto>>> GetPedidoBebidas(int idPedido)
    {
        if (_context.PedidoBebidas == null)
        {
            return NotFound();
        }

        // Aqui está a mágica:
        // 1. Filtramos os itens SÓ do pedido que queremos (Where)
        // 2. Fazemos o JOIN com a tabela Bebidas (Include)
        // 3. Criamos a DTO com os dados que queremos (Select)
        var pedidoBebidasDto = await _context.PedidoBebidas
            .Where(ph => ph.IdPedido == idPedido) // Filtra pelo pedido
            .Include(ph => ph.IdBebidasNavigation) // "JOIN" com Bebidas
            .Select(ph => new PedidoBebidasDto
            {
                Id = ph.Id,
                IdPedido = ph.IdPedido,
                IdBebidas = ph.IdBebidas,
                Quantidade = ph.Quantidade,
                PrecoUnitario = ph.IdBebidasNavigation!.Preco,
                NomeBebidas = ph.IdBebidasNavigation.Nome
            })
            .ToListAsync();

        return pedidoBebidasDto;
    }
}

// Coloque a classe DTO aqui no final do arquivo
public class PedidoBebidasDto
{
    public int? Id { get; set; }
    public int? IdPedido { get; set; }
    public int? IdBebidas { get; set; }
    public int? Quantidade { get; set; }
    public decimal? PrecoUnitario { get; set; }
    public string? NomeBebidas { get; set; }
}
