using System;
using System.Collections.Generic;

namespace TecBurguer.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public int IdUsuario { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cep { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
