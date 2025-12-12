using System.ComponentModel.DataAnnotations;

namespace TecBurguer.Models
{
    public partial class Bebidas
    {

        public Bebidas()
        {
            PedidoBebida = new HashSet<PedidoBebidas>();
        }

        public int IdBebidas { get; set; }

        [Required(ErrorMessage ="O campo NOME é obrigatório!")]
        public string? Nome { get; set; }

        public decimal? Preco { get; set; }

        public string? Imagem { get; set; }

        public int? Quantidade { get; set; }

        public virtual ICollection<PedidoBebidas> PedidoBebida { get; set; }
    }
}
