using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TecBurguer.Models
{
    public partial class Pedido
    {
        public Pedido()
        {
            Entregadors = new HashSet<Entregador>();
            PedidoHamburgueres = new HashSet<PedidoHamburguer>();
            PedidoBebida = new HashSet<PedidoBebidas>();
        }

        public int IdPedido { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal? PrecoTotal { get; set; }
        public string? Estado { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DataEntregue { get; set; }

        public int? IdUsuario { get; set; }

        public virtual Usuario? IdUsuarioNavigation { get; set; }
        public virtual ICollection<PedidoHamburguer> PedidoHamburgueres { get; set; }

        public virtual ICollection<PedidoBebidas> PedidoBebida { get; set; }
        public virtual ICollection<Entregador> Entregadors { get; set; }
    }
}