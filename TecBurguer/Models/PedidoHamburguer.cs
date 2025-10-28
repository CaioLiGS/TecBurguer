using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;


namespace TecBurguer.Models
{
    public partial class PedidoHamburguer
    {
        public int Id { get; set; }
        public int? IdHamburguer { get; set; }
        public int? IdPedido { get; set; }

        public int? Quantidade { get; set; }

        public virtual Hamburguer? IdHamburguerNavigation { get; set; }
        public virtual Pedido? IdPedidoNavigation { get; set; }
    }
}