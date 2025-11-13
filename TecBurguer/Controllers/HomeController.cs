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

        public IActionResult Entregadores()
        {
            var hamburgueres = _context.Hamburguers.ToList();
            return View(hamburgueres);
        }

        public IActionResult PedidoVendedor()
        {
            var pedidos = _context.Pedidos.Include(p => p.IdUsuarioNavigation);

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