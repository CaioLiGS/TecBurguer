namespace TecBurguer.Models
{
    public class PedidoBebidas
    {
        public int Id { get; set; }
        public int? IdBebidas { get; set; }
        public int? IdPedido { get; set; }
        public int? Quantidade { get; set; }
        public virtual Bebidas? IdBebidasNavigation { get; set; }
        public virtual Pedido? IdPedidoNavigation { get; set; }
    }
}
