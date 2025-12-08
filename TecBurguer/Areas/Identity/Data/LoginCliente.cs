using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TecBurguer.Areas.Identity.Data;

// Add profile data for application users by adding properties to the LoginCliente class
public class LoginCliente : IdentityUser
{
    public string Nome { get; set; }
    public bool Administrador { get; set; } = false;
    public bool Vendedor { get; set; } = false;

    public string Servico { get; set; } = "Usuário";
}

