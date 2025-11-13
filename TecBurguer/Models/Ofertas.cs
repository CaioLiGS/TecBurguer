using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TecBurguer.Models
{
    public partial class Ofertas
    {
        public int id { get; set; }
        public int? idHamburguer { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? dataTermino { get; set; }
        public decimal? precoFinal { get; set; }
        public decimal? porcentagem { get; set; }

        public virtual Hamburguer? idHamburguerNavigation { get; set; }
    }
}