using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public static class EmailService
{
    public static async Task SendAsync(string to, string subject, string body)
    {
        // Configurações do servidor SMTP
        var smtpHost = "smtp.gmail.com"; // Exemplo: smtp.gmail.com
        var smtpPort = 587; // Exemplo: 587 para TLS
        var smtpUser = "caiolugs@gmail.com";
        var smtpPass = "qozg kmvm ssev unbw";

        var fromAddress = new MailAddress(smtpUser, "TecBurguer");
        var toAddress = new MailAddress(to);

        using (var smtp = new SmtpClient(smtpHost, smtpPort))
        {
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}