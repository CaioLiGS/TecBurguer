using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TecBurguer.Models;
using System.Linq; // Necessário para o método DetalhesAPI

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly DBTecBurguerContext _context;

    public PedidosController(DBTecBurguerContext context)
    {
        _context = context;
    }

    // AÇÃO PARA RESOLVER O ERRO 404
    // Rota: /Pedido/DetalhesAPI/{id}
    [HttpGet("/Pedido/DetalhesAPI/{id}")]
    public async Task<IActionResult> DetalhesAPI(int id)
    {
        // 1. Buscar o pedido incluindo os itens relacionados (PedidoHamburgueres)
        // e os detalhes do Hambúrguer (IdHamburguerNavigation)
        var pedido = await _context.Pedidos
            .Include(p => p.PedidoHamburgueres)
                .ThenInclude(ph => ph.IdHamburguerNavigation)
            .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedido == null)
        {
            // Retorna 404 Not Found se o pedido não for encontrado
            return NotFound();
        }

        // 2. Mapeia os dados para um objeto anônimo (DTO) que a View Razor espera
        var resultado = new
        {
            pedido.IdPedido,
            pedido.PrecoTotal,
            pedido.Estado,
            // Mapeia os campos para o formato esperado pelo JavaScript:
            PedidoHamburgueres = pedido.PedidoHamburgueres.Select(ph => new
            {
                ph.Quantidade,
                hamburguerNome = ph.IdHamburguerNavigation.Nome,
                hamburguerPreco = ph.IdHamburguerNavigation.Preco
            }).ToList()
        };

        // 3. Retorna o objeto mapeado como JSON com status 200 OK
        return Ok(resultado);
    }

    // --- Suas Actions existentes continuam abaixo ---

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

    // PATCH: api/Pedidos/update/5
    [HttpPatch("update/{id}")]
    public async Task<IActionResult> PatchPedido(int id, [FromBody] Pedido dadosParciais)
    {
        // 1. Encontrar o pedido existente no banco
        var pedidoDoBanco = await _context.Pedidos
        .Include(p => p.PedidoHamburgueres)
            .ThenInclude(ph => ph.IdHamburguerNavigation)
                .ThenInclude(h => h!.HamburguerIgredientes)
                    .ThenInclude(hi => hi.IdIngredienteNavigation)
        .Include(b => b.PedidoBebida)
            .ThenInclude(beb => beb.IdBebidasNavigation)
        .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedidoDoBanco == null)
        {
            return NotFound();
        }

        if (dadosParciais.Estado == "Para ser entregue" && pedidoDoBanco.Estado != "Para ser entregue")
        {
            foreach (var itemPedido in pedidoDoBanco.PedidoHamburgueres)
            {
                if (itemPedido.IdHamburguerNavigation?.HamburguerIgredientes == null) continue;

                int qtdLanches = itemPedido.Quantidade ?? 0;

                foreach (var receita in itemPedido.IdHamburguerNavigation.HamburguerIgredientes)
                {
                    var ingrediente = receita.IdIngredienteNavigation;
                    int qtdPorLanche = receita.QuantidadeNecessario ?? 0;

                    if (ingrediente != null)
                    {
                        int totalDeduzir = qtdLanches * qtdPorLanche;
                        ingrediente.Quantidade -= totalDeduzir;

                        if (ingrediente.Quantidade < 0) return BadRequest($"Falta estoque de {ingrediente.Nome}");

                        _context.Entry(ingrediente).State = EntityState.Modified;
                    }
                }
            }

            foreach (var itemBebida in pedidoDoBanco.PedidoBebida)
            {
                itemBebida.IdBebidasNavigation!.Quantidade -= itemBebida.Quantidade ?? 0;

                if (itemBebida.IdBebidasNavigation.Quantidade < 0) return BadRequest($"Falta estoque de {itemBebida.IdBebidasNavigation.Nome}");

                _context.Entry(itemBebida.IdBebidasNavigation).State = EntityState.Modified;
            }
        }
        // 2. Aplicar manualmente as atualizações parciais
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

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
        {
            return NotFound();
        }

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PedidoExists(int id)
    {
        return (_context.Pedidos?.Any(e => e.IdPedido == id)).GetValueOrDefault();
    }
}