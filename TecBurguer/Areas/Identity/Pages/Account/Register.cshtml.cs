// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TecBurguer.Areas.Identity.Data;
using TecBurguer.Models;

namespace TecBurguer.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<LoginCliente> _signInManager;
        private readonly UserManager<LoginCliente> _userManager;
        private readonly IUserStore<LoginCliente> _userStore;
        private readonly IUserEmailStore<LoginCliente> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DBTecBurguerContext _dbTecBurguerContext;

        public RegisterModel(
            UserManager<LoginCliente> userManager,
            IUserStore<LoginCliente> userStore,
            SignInManager<LoginCliente> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            DBTecBurguerContext dBTecBurguerContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbTecBurguerContext = dBTecBurguerContext;
        }

        [BindProperty]
        public string ConfirmationCode { get; set; }

        public string GeneratedCode { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage ="O campo Email é obrigatório.")]
            [EmailAddress(ErrorMessage ="Por favor, insira um email válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "O campo Nome é obrigatório.")]
            [Display(Name = "Nome")]
            public string Nome { get; set; }


            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "O campo Senha é obrigatório.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "A senha e a confirmação não estão identicas.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.Nome = Input.Nome;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "Usuário");

                    _dbTecBurguerContext.Usuarios.Add(new Usuario
                    {
                        Nome = Input.Nome,
                        Email = Input.Email
                    });

                    await _dbTecBurguerContext.SaveChangesAsync();

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    GeneratedCode = new Random().Next(100000, 999999).ToString();

                    string emailSubject = "🔥 TecBurguer: Seu código de acesso";

                    string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        /* Estilos básicos para clientes de e-mail que suportam */
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background-color: #000000; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #121212; border-radius: 15px; overflow: hidden; }}
        .header {{ background: linear-gradient(45deg, #FF2E00, #d62500); padding: 30px; text-align: center; }}
        .content {{ padding: 40px 30px; color: #f5f5f5; text-align: center; }}
        .code-box {{ 
            background-color: rgba(255, 46, 0, 0.1); 
            border: 2px solid #FF2E00; 
            border-radius: 10px; 
            padding: 20px; 
            font-size: 32px; 
            font-weight: bold; 
            letter-spacing: 5px; 
            color: #FF2E00; 
            margin: 30px 0;
            display: inline-block;
            min-width: 200px;
        }}
        .footer {{ background-color: #0a0a0a; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
        .btn-link {{ color: #FFC107; text-decoration: none; }}
    </style>
</head>
<body style='margin: 0; padding: 20px; background-color: #000000;'>
    
    <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: #121212; border-radius: 15px; overflow: hidden; box-shadow: 0 0 20px rgba(255, 46, 0, 0.2);'>
        
        <tr>
            <td align='center' style='padding: 30px; background: linear-gradient(45deg, #FF2E00, #991b00);'>
                <h1 style='color: #ffffff; margin: 0; font-family: Impact, sans-serif; letter-spacing: 2px; font-size: 28px;'>TEC BURGER</h1>
            </td>
        </tr>

        <tr>
            <td align='center' style='padding: 40px 30px; color: #eeeeee; font-family: sans-serif;'>
                <h2 style='margin-top: 0; font-weight: normal;'>Olá!</h2>
                <p style='font-size: 16px; line-height: 1.5; color: #cccccc;'>
                    Recebemos uma solicitação de acesso para sua conta. <br>
                    Use o código abaixo para continuar:
                </p>

                <div style='margin: 30px 0;'>
                    <span style='
                        display: inline-block;
                        background-color: #1a0500;
                        border: 2px dashed #FF2E00;
                        border-radius: 12px;
                        padding: 15px 40px;
                        font-size: 36px;
                        font-weight: bold;
                        letter-spacing: 8px;
                        color: #FF2E00;
                    '>
                        {GeneratedCode}
                    </span>
                </div>

                <p style='font-size: 14px; color: #888888;'>
                    Este código expira em 10 minutos.<br>
                    Se você não solicitou este código, pode ignorar este e-mail com segurança.
                </p>
            </td>
        </tr>

        <tr>
            <td align='center' style='padding: 20px; background-color: #080808; color: #555555; font-family: sans-serif; font-size: 12px; border-top: 1px solid #222;'>
                <p style='margin: 0;'>&copy; {DateTime.Now.Year} TecBurguer. O sabor do futuro.</p>
                <p style='margin: 5px 0 0 0;'>
                    Precisa de ajuda? <a href='#' style='color: #FFC107; text-decoration: none;'>Fale conosco</a>
                </p>
            </td>
        </tr>
    </table>

</body>
</html>
";

                    // 3. Enviar e-mail
                    await EmailService.SendAsync(Input.Email, emailSubject, emailBody);

                    // Salvar código em sessão ou banco (exemplo: sessão)
                    HttpContext.Session.SetString("ConfirmationCode", GeneratedCode);
                    HttpContext.Session.SetString("PendingEmail", Input.Email);

                    // Exibir campo para digitar o código
                    

                    /*await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        ViewData["ShowConfirmation"] = true;

                        // return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private LoginCliente CreateUser()
        {
            try
            {
                return Activator.CreateInstance<LoginCliente>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(LoginCliente)}'. " +
                    $"Ensure that '{nameof(LoginCliente)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<LoginCliente> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<LoginCliente>)_userStore;
        }



        public async Task<IActionResult> OnPostConfirmAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var code = HttpContext.Session.GetString("ConfirmationCode");
            var email = HttpContext.Session.GetString("PendingEmail");

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Sessão expirada. Por favor, refaça o cadastro.");
                return Page();
            }

            if (ConfirmationCode == code)
            {
                // Prosseguir com o cadastro normalmente
                HttpContext.Session.Remove("ConfirmationCode");
                HttpContext.Session.Remove("PendingEmail");

                // Buscar o usuário pelo e-mail
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }


                ViewData["ConfirmationSuccess"] = true;

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redireciona para a URL passada ou para home
                return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError("", "Código inválido.");
            ViewData["ShowConfirmation"] = true;
            return Page();
        }
    }
}
