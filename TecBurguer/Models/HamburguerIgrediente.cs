using System;
using System.Collections.Generic;

namespace TecBurguer.Models
{
    public partial class HamburguerIgrediente
    {
        public int Id { get; set; }
        public int? IdHamburguer { get; set; }
        public int? IdIngrediente { get; set; }
        public int? QuantidadeNecessario { get; set; }

        public virtual Hamburguer? IdHamburguerNavigation { get; set; }
        public virtual Ingrediente? IdIngredienteNavigation { get; set; }
    }
}
