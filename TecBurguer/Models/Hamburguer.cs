using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TecBurguer.Models
{
    public partial class Hamburguer
    {
        public Hamburguer()
        {
            HamburguerIgredientes = new HashSet<HamburguerIgrediente>();
            PedidoHamburgueres = new HashSet<PedidoHamburguer>();
        }

        public int IdHamburguer { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        public decimal? Preco { get; set; }
        public string? Imagem { get; set; }
        public string? Categoria { get; set; }

        public virtual ICollection<HamburguerIgrediente> HamburguerIgredientes { get; set; }
        public virtual ICollection<PedidoHamburguer> PedidoHamburgueres { get; set; }
    }
}