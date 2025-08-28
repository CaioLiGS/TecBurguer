using System;
using System.Collections.Generic;

namespace TecBurguer.Models
{
    public partial class Pedido
    {
        public Pedido()
        {
            Entregadors = new HashSet<Entregador>();
        }

        public int IdPedido { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public byte[] DataHorario { get; set; } = null!;
        public decimal? PrecoTotal { get; set; }
        public string? Estado { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdHamburguer { get; set; }

        public virtual Hamburguer? IdHamburguerNavigation { get; set; }
        public virtual Usuario? IdUsuarioNavigation { get; set; }
        public virtual ICollection<Entregador> Entregadors { get; set; }
    }
}
