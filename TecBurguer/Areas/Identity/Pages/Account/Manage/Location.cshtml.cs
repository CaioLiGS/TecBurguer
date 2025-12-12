using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Importante para o FirstOrDefaultAsync
using TecBurguer.Areas.Identity.Data;
using TecBurguer.Models; // Importante: Namespace do seu Contexto e Model Usuario

namespace TecBurguer.Areas.Identity.Pages.Account.Manage
{
    public class LocationModel : PageModel
    {
        private readonly UserManager<LoginCliente> _userManager;
        private readonly SignInManager<LoginCliente> _signInManager;
        private readonly DBTecBurguerContext _context; // Seu Contexto de Banco de Dados

        public LocationModel(
            UserManager<LoginCliente> userManager,
            SignInManager<LoginCliente> signInManager,
            DBTecBurguerContext context) // Injeção do Contexto
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "CEP")]
            [Required(ErrorMessage ="O campo CEP é obrigatório!")]
            public string Cep { get; set; }

            [Display(Name = "Rua")]
            [Required(ErrorMessage = "O campo RUA é obrigatório!")]
            public string Rua { get; set; }

            [Display(Name = "Bairro")]
            [Required(ErrorMessage = "O campo BAIRRO é obrigatório!")]
            public string Bairro { get; set; }

            [Display(Name = "Cidade")]
            [Required(ErrorMessage = "O campo CIDADE é obrigatório!")]
            public string Cidade { get; set; }

            [Display(Name = "Estado")]
            [Required(ErrorMessage = "O campo ESTADO é obrigatório!")]
            public string Estado { get; set; }
        }

        private async Task LoadAsync(Usuario usuario)
        {
            Input = new InputModel
            {
                Cep = usuario.Cep,
                Rua = usuario.Rua,
                Bairro = usuario.Bairro,
                Cidade = usuario.Cidade,
                Estado = usuario.Estado
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userLogin = await _userManager.GetUserAsync(User);
            if (userLogin == null)
            {
                return NotFound($"Não foi possível carregar o usuário com a id '{_userManager.GetUserId(User)}'.");
            }

            // BUSCA O USUÁRIO DE NEGÓCIO PELO EMAIL DO LOGIN
            var usuarioNegocio = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == userLogin.UserName);

            if (usuarioNegocio == null)
            {
                // Se não achar, cria um Input vazio ou trata o erro
                Input = new InputModel();
                StatusMessage = "Atenção: Perfil de usuário não encontrado para este login.";
            }
            else
            {
                await LoadAsync(usuarioNegocio);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userLogin = await _userManager.GetUserAsync(User);
            if (userLogin == null)
            {
                return NotFound($"Não foi possível carregar o usuário com a id:  '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                // Recarrega dados se o form for inválido
                var usuarioRecarregar = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userLogin.UserName);
                if (usuarioRecarregar != null) await LoadAsync(usuarioRecarregar);
                return Page();
            }

            // BUSCA O USUÁRIO PARA ATUALIZAR
            var usuarioNegocio = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == userLogin.UserName);

            if (usuarioNegocio == null)
            {
                StatusMessage = "Erro: Usuário de negócio não encontrado.";
                return RedirectToPage();
            }

            // Atualiza as propriedades do Usuario (TecBurguer.Models.Usuario)
            usuarioNegocio.Cep = Input.Cep;
            usuarioNegocio.Rua = Input.Rua;
            usuarioNegocio.Bairro = Input.Bairro;
            usuarioNegocio.Cidade = Input.Cidade;
            usuarioNegocio.Estado = Input.Estado;

            // Salva no banco de dados da aplicação
            _context.Usuarios.Update(usuarioNegocio);
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(userLogin);
            StatusMessage = "Endereço atualizado com sucesso!";
            return RedirectToPage();
        }
    }
}