// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TecBurguer.Areas.Identity.Data;

namespace TecBurguer.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<LoginCliente> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<LoginCliente> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = new Random().Next(100000, 999999).ToString();

                await _userManager.SetAuthenticationTokenAsync(user, "TecBurguer", "ResetCode", code);

                string emailSubject = "🔐 TecBurguer: Redefinição de Senha";

                string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background-color: #000000; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #121212; border-radius: 15px; overflow: hidden; }}
    </style>
</head>
<body style='margin: 0; padding: 20px; background-color: #000000;'>
    
    <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: #121212; border-radius: 15px; overflow: hidden; box-shadow: 0 0 25px rgba(255, 46, 0, 0.25); border: 1px solid #333;'>
        
        <tr>
            <td align='center' style='padding: 35px; background: linear-gradient(135deg, #FF2E00, #801700);'>
                <h1 style='color: #ffffff; margin: 0; font-family: Arial, sans-serif; font-weight: 900; letter-spacing: 1px; font-size: 26px; text-transform: uppercase;'>
                    RECUPERAÇÃO DE CONTA
                </h1>
            </td>
        </tr>

        <tr>
            <td align='center' style='padding: 40px 30px; color: #e0e0e0; font-family: sans-serif;'>
                <h2 style='margin-top: 0; font-weight: normal; color: #ffffff;'>Esqueceu a senha?</h2>
                <p style='font-size: 16px; line-height: 1.6; color: #bbbbbb; margin-bottom: 25px;'>
                    Não se preocupe! Recebemos uma solicitação para redefinir a senha da sua conta <strong>TecBurguer</strong>.
                </p>
                <p style='font-size: 16px; line-height: 1.6; color: #bbbbbb;'>
                    Use o código de segurança abaixo para prosseguir:
                </p>

                <div style='margin: 35px 0;'>
                    <span style='
                        display: inline-block;
                        background-color: rgba(255, 255, 255, 0.05);
                        border: 2px solid #FF2E00;
                        border-radius: 8px;
                        padding: 15px 50px;
                        font-size: 38px;
                        font-weight: bold;
                        letter-spacing: 10px;
                        color: #FF2E00;
                        box-shadow: 0 0 15px rgba(255, 46, 0, 0.2);
                        text-shadow: 0 0 5px rgba(255, 46, 0, 0.5);
                    '>
                        {code}
                    </span>
                </div>

                <p style='font-size: 13px; color: #888888; border-top: 1px solid #333; padding-top: 20px; margin-top: 30px;'>
                    ⚠️ <strong>Importante:</strong> Este código expira em breve.<br>
                    Se você não solicitou essa alteração, ignore este e-mail. Sua senha permanecerá a mesma e sua conta está segura.
                </p>
            </td>
        </tr>

        <tr>
            <td align='center' style='padding: 20px; background-color: #080808; color: #555555; font-family: sans-serif; font-size: 12px;'>
                <p style='margin: 0;'>&copy; {DateTime.Now.Year} TecBurguer</p>
                <p style='margin: 5px 0 0 0;'>Enviado automaticamente pelo sistema de segurança.</p>
            </td>
        </tr>
    </table>

</body>
</html>
";

                await EmailService.SendAsync(Input.Email, emailSubject, emailBody);

                return RedirectToPage("./VerifyResetCode", new { email = Input.Email });
            }

            return Page();
        }
    }
}
