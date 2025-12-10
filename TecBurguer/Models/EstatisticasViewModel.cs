using System.Collections.Generic;

namespace TecBurguer.Models
{
    public class EstatisticasViewModel
    {
        // Totais Financeiros
        public decimal VendasHoje { get; set; }
        public decimal VendasSemana { get; set; }
        public decimal VendasMes { get; set; }
        public decimal VendasAno { get; set; }

        // Top Burguer
        public string TopHamburguerNome { get; set; }
        public int TopHamburguerQtd { get; set; }
        public decimal TopHamburguerRendimento { get; set; }

        // Gráficos de Evolução
        public List<VendaPeriodo> GraficoDiario { get; set; } // Últimos 7 dias
        public List<VendaPeriodo> GraficoSemanal { get; set; } // Semanas do Mês atual
        public List<VendaPeriodo> GraficoMensal { get; set; }  // Meses do Ano atual

        // Estatísticas por Categoria
        public List<CategoriaStats> EstatisticasPorCategoria { get; set; }

        // NOVO: Relatório de Ingredientes e Reposição
        public List<IngredienteRelatorio> RelatorioIngredientes { get; set; }
    }

    public class VendaPeriodo
    {
        public string Label { get; set; } // Ex: "Segunda", "Semana 1", "Janeiro"
        public decimal Total { get; set; }
    }

    public class CategoriaStats
    {
        public string NomeCategoria { get; set; }
        public decimal MediaVendas { get; set; }
        public decimal TotalVendas { get; set; }
    }

    public class IngredienteRelatorio
    {
        public string NomeIngrediente { get; set; }
        public int GastoTotal { get; set; }     // Gasto histórico total (opcional)
        public int GastoSemana { get; set; }    // O foco para reposição
        public int QuantidadeTotal { get; set; }

        public bool AlertaReposicao { get; set; } // True se o consumo for alto
    }
}