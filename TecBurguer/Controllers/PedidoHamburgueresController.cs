
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

        return await _context.PedidoHamburgueres.ToListAsync();
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdatePedidoHamburguer(int id, [FromBody] PedidoHamburguer pedidoHamburguer)
    {
        if (id != pedidoHamburguer.Id) return BadRequest();

        var itemExistente = await _context.PedidoHamburgueres.FindAsync(id);
        if (itemExistente == null) return NotFound();

        var hamburguer = await _context.Hamburguers
            .Include(h => h.HamburguerIgredientes)
            .ThenInclude(hi => hi.IdIngredienteNavigation)
            .FirstOrDefaultAsync(h => h.IdHamburguer == pedidoHamburguer.IdHamburguer);

        if (pedidoHamburguer.Quantidade > itemExistente.Quantidade)
        {
            foreach (var receita in hamburguer.HamburguerIgredientes)
            {
                var ingrediente = receita.IdIngredienteNavigation;
                int qtdPorLanche = receita.QuantidadeNecessario ?? 0;
                int novaQtdTotalLanches = pedidoHamburguer.Quantidade ?? 0;

                if (ingrediente != null)
                {
                    int totalIngredienteNecessario = novaQtdTotalLanches * qtdPorLanche;

                    if (totalIngredienteNecessario > ingrediente.Quantidade)
                    {
                        return BadRequest($"Estoque insuficiente de {ingrediente.Nome} para adicionar mais itens!");
                    }
                }
            }
        }

        _context.Entry(itemExistente).State = EntityState.Detached;
        _context.Entry(pedidoHamburguer).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PedidoHamburguerExists(id)) return NotFound();
            else throw;
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
    [HttpGet("ListarPorPedido/{idPedido}")]
    public async Task<ActionResult<IEnumerable<PedidoHamburguerDto>>> GetPedidoHamburgueres(int idPedido)
    {
        if (_context.PedidoHamburgueres == null)
        {
            return NotFound();
        }

        // Aqui está a mágica:
        // 1. Filtramos os itens SÓ do pedido que queremos (Where)
        // 2. Fazemos o JOIN com a tabela Hamburgueres (Include)
        // 3. Criamos a DTO com os dados que queremos (Select)
        var pedidoHamburgueresDto = await _context.PedidoHamburgueres
            .Where(ph => ph.IdPedido == idPedido) // Filtra pelo pedido
            .Include(ph => ph.IdHamburguerNavigation) // "JOIN" com Hamburguer
            .Select(ph => new PedidoHamburguerDto
            {
                Id = ph.Id,
                IdPedido = ph.IdPedido,
                IdHamburguer = ph.IdHamburguer,
                Quantidade = ph.Quantidade,
                PrecoUnitario = ph.IdHamburguerNavigation!.Preco,
                NomeHamburguer = ph.IdHamburguerNavigation.Nome
            })
            .ToListAsync();

        return pedidoHamburgueresDto;
    }
}

// Coloque a classe DTO aqui no final do arquivo
public class PedidoHamburguerDto
{
    public int? Id { get; set; }
    public int? IdPedido { get; set; }
    public int? IdHamburguer { get; set; }
    public int? Quantidade { get; set; }
    public decimal? PrecoUnitario { get; set; }
    public string? NomeHamburguer { get; set; }
}
