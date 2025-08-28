using System;
using System.Collections.Generic;

namespace TecBurguer.Models
{
    public partial class Ingrediente
    {
        public Ingrediente()
        {
            HamburguerIgredientes = new HashSet<HamburguerIgrediente>();
        }

        public int IdIngrediente { get; set; }
        public string? Nome { get; set; }
        public int? Quantidade { get; set; }

        public virtual ICollection<HamburguerIgrediente> HamburguerIgredientes { get; set; }
    }
}
