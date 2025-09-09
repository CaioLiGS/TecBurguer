using System;
using System.Collections.Generic;

namespace TecBurguer.Models
{
    public partial class Entregador
    {
        public int IdEntregador { get; set; }
        public string Nome { get; set; }
        public decimal Avaliacao { get; set; }
        public string Veiculo { get; set; }
        public int IdPedido { get; set; }

        public virtual Pedido? IdPedidoNavigation { get; set; }
    }
    
}
