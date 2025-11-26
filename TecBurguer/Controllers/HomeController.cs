using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBTecBurguerContext _context;

        public HomeController(ILogger<HomeController> logger, DBTecBurguerContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var hamburgueres = _context.Hamburguers.Include(h => h.HamburguerIgredientes)       
                    .ThenInclude(hi => hi.IdIngredienteNavigation)
                    .ToList();

            var agora = DateTime.Now;

            var ofertasAtivas = _context.Ofertas
                .Include(o => o.idHamburguerNavigation) 
                .Where(o => o.dataTermino.HasValue && o.dataTermino > agora)
                .ToList();

            ViewData["OfertasAtivas"] = ofertasAtivas;

            return View(hamburgueres);
        }

        public IActionResult Cart()
        {
            var pedidos = _context.Pedidos
                .Include(p => p.PedidoHamburgueres)
                .ThenInclude(ham => ham.IdHamburguerNavigation)
                .Include(u => u.IdUsuarioNavigation);

            return View(pedidos.ToList());
        }

        public IActionResult Cardapio()
        {
            var hamburgueres = _context.Hamburguers.ToList();
            return View(hamburgueres); 
        }

        // ==========================================
        // ÁREA DOS ENTREGADORES
        // ==========================================

        // GET: Exibe a lista de pedidos disponíveis ("Para ser entregue")
        public IActionResult Entregadores()
        {
            var pedidosDisponiveis = _context.Pedidos
                .Include(p => p.IdUsuarioNavigation) // Inclui dados do cliente para ver endereço/nome
                .Where(p => p.Estado == "Para ser entregue")
                .ToList();

            return View(pedidosDisponiveis);
        }

        // POST: Processa a escolha do pedido pelo entregador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarEntrega(int idPedido)
        {
            var pedido = await _context.Pedidos.FindAsync(idPedido);

            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.Estado != "Para ser entregue")
            {
                TempData["Erro"] = "Este pedido já foi pego por outro entregador!";
                return RedirectToAction(nameof(Entregadores));
            }

            pedido.Estado = "Saiu para entrega";
            _context.Update(pedido);

            var novaEntrega = new Entregador
            {
                IdPedido = pedido.IdPedido,
                Nome = "Entregador da Sessão",
                Veiculo = "Moto",
                Avaliacao = 5.0m               
            };

            _context.Add(novaEntrega);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Você aceitou o pedido #{pedido.IdPedido}. Boa entrega!";
            return RedirectToAction(nameof(Entregadores));
        }


        public IActionResult PedidoVendedor()
        {
            var pedidos = _context.Pedidos.Include(p => p.IdUsuarioNavigation)
                .Include(d => d.PedidoHamburgueres)
                .ThenInclude(ph => ph.IdHamburguerNavigation)
                    .ThenInclude(h => h!.HamburguerIgredientes)
                        .ThenInclude(hi => hi.IdIngredienteNavigation);

            return View(pedidos.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}