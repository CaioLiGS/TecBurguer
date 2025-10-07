using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TecBurguer.Areas.Identity.Data;

namespace TecBurguer.Areas.Identity.Pages.Account
{
    public class VerifyResetCodeModel : PageModel
    {
        private readonly UserManager<LoginCliente> _userManager;

        public VerifyResetCodeModel(UserManager<LoginCliente> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Code { get; set; }

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Code))
            {
                ModelState.AddModelError("", "Email ou código não informado.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado.");
                return Page();
            }

            var savedCode = await _userManager.GetAuthenticationTokenAsync(user, "TecBurguer", "ResetCode");

            if (savedCode == Code)
            {
                // Código válido → redireciona para redefinir senha
                return RedirectToPage("./ResetPassword", new { email = Email });
            }

            ModelState.AddModelError("", "Código inválido. Verifique e tente novamente.");
            return Page();
        }
    }
}
