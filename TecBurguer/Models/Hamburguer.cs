using System;
using System.Collections.Generic;

namespace TecBurguer.Models
{
    public partial class Hamburguer
    {
        public Hamburguer()
        {
            HamburguerIgredientes = new HashSet<HamburguerIgrediente>();
            Pedidos = new HashSet<Pedido>();
        }

        public int IdHamburguer { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal? Preco { get; set; }
        public string? Imagem { get; set; }

        public virtual ICollection<HamburguerIgrediente> HamburguerIgredientes { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
