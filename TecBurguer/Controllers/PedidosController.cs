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
        var pedidoDoBanco = await _context.Pedidos.FindAsync(id);
        if (pedidoDoBanco == null)
        {
            return NotFound();
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

    private bool PedidoExists(int id)
    {
        return (_context.Pedidos?.Any(e => e.IdPedido == id)).GetValueOrDefault();
    }
}