using Microsoft.AspNetCore.Mvc;
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
        // 1. Verifica se os dados do pedido são válidos (validação automática do ASP.NET Core)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Retorna 400 se os dados não forem válidos
        }

        // 2. Adiciona o pedido ao contexto do Entity Framework
        _context.Pedidos.Add(pedido);

        // 3. Salva a nova entrada no banco de dados
        await _context.SaveChangesAsync();

        // 4. Retorna uma resposta de sucesso com o ID do novo pedido
        return CreatedAtAction(nameof(Create), new { id = pedido.IdPedido }, pedido);
    }
}