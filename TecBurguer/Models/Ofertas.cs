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
        [Required(ErrorMessage ="O campo DATA DE TÉRMINO é obrigatório!")]

        [DataType(DataType.DateTime)]
        public DateTime? dataTermino { get; set; }
        [Required(ErrorMessage = "O campo PREÇO FINAL é obrigatório!")]
        public decimal? precoFinal { get; set; }

        [Required(ErrorMessage = "O campo PORCENTAGEM é obrigatório!")]
        public decimal? porcentagem { get; set; }

        public virtual Hamburguer? idHamburguerNavigation { get; set; }
    }
}