using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecBurguer.Areas.Identity.Data;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBTecBurguerContext _context;
        private readonly LoginContext _loginContext;

        public HomeController(ILogger<HomeController> logger, DBTecBurguerContext context, LoginContext loginContext)
        {
            _logger = logger;
            _context = context;
            _loginContext = loginContext;
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

        // ==========================================
        // ÁREA DOS ENTREGADORES
        // ==========================================

        // GET: Exibe a lista de pedidos disponíveis ("Para ser entregue")
        public IActionResult Entregadores()
        {
            var pedidosDisponiveis = _context.Pedidos
                .Include(p => p.IdUsuarioNavigation)
                .Where(p => p.Estado == "Para ser entregue" || p.Estado == "Saiu para entrega")
                .Include(p => p.Entregadors)
                .ToList();

            return View(pedidosDisponiveis);
        }

        // POST: Processa a escolha do pedido pelo entregador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarEntrega(int idPedido, string NomeEntregador)
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
                Nome = NomeEntregador,
                Veiculo = "Moto",
                Avaliacao = 5.0m               
            };

            _context.Add(novaEntrega);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Você aceitou o pedido #{pedido.IdPedido}. Boa entrega!";
            return RedirectToAction(nameof(Entregadores));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarEntrega(int idPedido)
        {
            var pedido = await _context.Pedidos.FindAsync(idPedido);

            if (pedido == null)
            {
                TempData["Erro"] = "Pedido não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                pedido.Estado = "Entregue";
                pedido.IdUsuario = null;
                pedido.DataEntregue = DateTime.Now;

                _context.Update(pedido);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = $"Entrega do pedido #{idPedido} confirmada!";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao finalizar entrega: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> UsuariosContrato()
        {
            var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();
            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarServico(int idUsuario, string novoServico)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);

            if (usuario != null)
            {
                usuario.Servico = novoServico;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(usuario.Email))
                {
                    var usuarioIdentity = await _loginContext.Users
                        .FirstOrDefaultAsync(u => u.Email == usuario.Email);

                    if (usuarioIdentity != null)
                    {
                        usuarioIdentity.Servico = novoServico;

                        _loginContext.Update(usuarioIdentity);
                        await _loginContext.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(UsuariosContrato));
        }
        public async Task<IActionResult> Estatisticas()
        {
            var agora = DateTime.Now;
            var hoje = agora.Date;
            var inicioSemana = hoje.AddDays(-7); // Considerando janela de 7 dias
            var inicioMes = new DateTime(agora.Year, agora.Month, 1);
            var inicioAno = new DateTime(agora.Year, 1, 1);

            // Carrega pedidos entregues com todas as dependências de ingredientes
            var pedidos = await _context.Pedidos
                .Include(p => p.PedidoHamburgueres)
                    .ThenInclude(ph => ph.IdHamburguerNavigation)
                        .ThenInclude(h => h.HamburguerIgredientes)
                            .ThenInclude(hi => hi.IdIngredienteNavigation) // Importante carregar o nome do ingrediente
                .Where(p => p.Estado == "Entregue" && p.DataEntregue.HasValue)
                .ToListAsync();

            var vm = new EstatisticasViewModel();

            // 1. Cards Totais
            vm.VendasHoje = pedidos.Where(p => p.DataEntregue.Value.Date == hoje).Sum(p => p.PrecoTotal ?? 0);
            vm.VendasSemana = pedidos.Where(p => p.DataEntregue.Value.Date >= inicioSemana).Sum(p => p.PrecoTotal ?? 0);
            vm.VendasMes = pedidos.Where(p => p.DataEntregue.Value.Date >= inicioMes).Sum(p => p.PrecoTotal ?? 0);
            vm.VendasAno = pedidos.Where(p => p.DataEntregue.Value.Date >= inicioAno).Sum(p => p.PrecoTotal ?? 0);

            // 2. Hambúrguer Mais Vendido
            var itensVendidos = pedidos.SelectMany(p => p.PedidoHamburgueres).ToList();
            var topBurguer = itensVendidos
                .GroupBy(ph => ph.IdHamburguerNavigation)
                .OrderByDescending(g => g.Sum(ph => ph.Quantidade ?? 0))
                .FirstOrDefault();

            if (topBurguer != null)
            {
                vm.TopHamburguerNome = topBurguer.Key.Nome;
                vm.TopHamburguerQtd = topBurguer.Sum(ph => ph.Quantidade ?? 0);
                vm.TopHamburguerRendimento = topBurguer.Sum(ph => (ph.Quantidade ?? 0) * (topBurguer.Key.Preco ?? 0));
            }

            // 3. Gráficos de Evolução

            // A) Diária (Últimos 7 dias)
            vm.GraficoDiario = pedidos
                .Where(p => p.DataEntregue.Value.Date >= hoje.AddDays(-6))
                .GroupBy(p => p.DataEntregue.Value.Date)
                .Select(g => new VendaPeriodo { Label = g.Key.ToString("dd/MM"), Total = g.Sum(p => p.PrecoTotal ?? 0) })
                .OrderBy(x => x.Label)
                .ToList();

            // B) Semanal (Dentro do Mês Atual)
            vm.GraficoSemanal = pedidos
                .Where(p => p.DataEntregue.Value.Date >= inicioMes)
                .GroupBy(p => GetWeekOfMonth(p.DataEntregue.Value))
                .Select(g => new VendaPeriodo { Label = "Semana " + g.Key, Total = g.Sum(p => p.PrecoTotal ?? 0) })
                .OrderBy(x => x.Label)
                .ToList();

            // C) Mensal (Dentro do Ano Atual)
            vm.GraficoMensal = pedidos
                .Where(p => p.DataEntregue.Value.Date >= inicioAno)
                .GroupBy(p => p.DataEntregue.Value.Month)
                .Select(g => new VendaPeriodo
                {
                    Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                    Total = g.Sum(p => p.PrecoTotal ?? 0)
                })
                .ToList(); // A ordenação pode precisar de ajuste dependendo se o mês vem como int ou string, aqui assume ordem de inserção

            // 4. Categorias
            vm.EstatisticasPorCategoria = itensVendidos
                .GroupBy(ph => ph.IdHamburguerNavigation.Categoria)
                .Select(g => new CategoriaStats
                {
                    NomeCategoria = g.Key ?? "Geral",
                    TotalVendas = g.Sum(ph => (ph.Quantidade ?? 0) * (ph.IdHamburguerNavigation.Preco ?? 0)),
                    MediaVendas = g.Average(ph => (ph.Quantidade ?? 0) * (ph.IdHamburguerNavigation.Preco ?? 0))
                }).ToList();

            // 5. Relatório de Ingredientes e Alerta de Reposição
            // Lógica: Calcula quanto de ingrediente foi gasto nos pedidos dos últimos 7 dias
            var pedidosRecentes = pedidos.Where(p => p.DataEntregue.Value >= inicioSemana).ToList();

            var usoIngredientes = new List<IngredienteRelatorio>();

            // Iterar sobre todos os pedidos recentes -> hambúrgueres -> ingredientes
            foreach (var pedido in pedidosRecentes)
            {
                foreach (var item in pedido.PedidoHamburgueres)
                {
                    int qtdLanche = item.Quantidade ?? 0;
                    foreach (var ingr in item.IdHamburguerNavigation.HamburguerIgredientes)
                    {
                        var nomeIng = ingr.IdIngredienteNavigation?.Nome ?? "Ingrediente Desc.";
                        var qtdGasta = qtdLanche * (ingr.QuantidadeNecessario ?? 0);

                        var existente = usoIngredientes.FirstOrDefault(i => i.NomeIngrediente == nomeIng);
                        if (existente == null)
                        {
                            usoIngredientes.Add(new IngredienteRelatorio { NomeIngrediente = nomeIng, GastoSemana = qtdGasta, QuantidadeTotal = ingr.IdIngredienteNavigation?.Quantidade ?? 0});
                        }
                        else
                        {
                            existente.GastoSemana += qtdGasta;
                        }
                    }
                }
            }

            // Definição de Regra de Alerta: Se gastou mais de 50 unidades na semana, sugere reposição.
            // (Você pode ajustar esse valor '50' conforme a realidade do negócio)
            foreach (var item in usoIngredientes)
            {
                item.AlertaReposicao = item.GastoSemana >= item.QuantidadeTotal;
            }

            vm.RelatorioIngredientes = usoIngredientes.OrderByDescending(i => i.GastoSemana).ToList();

            return View(vm);
        }

        // Helper para pegar número da semana dentro do mês
        private int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            int offset = (int)beginningOfMonth.DayOfWeek; // Domingo = 0
            return (date.Day + offset - 1) / 7 + 1;
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