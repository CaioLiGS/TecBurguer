using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TecBurguer.Models;

[Authorize(Roles = "Administrador")]
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

        return CreatedAtAction(nameof(Create), new { id = pedido.IdPedido }, pedido);
    }
}