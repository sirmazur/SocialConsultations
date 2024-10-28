using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Net;

namespace SocialConsultations.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("mazur123212345@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage};

            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587,
                    MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("mazur123212345@gmail.com", _configuration["MailSettings:Password"]);
                await emailClient.SendAsync(emailToSend);
            }

            //var client = new System.Net.Mail.SmtpClient("smtp.mailersend.net", 587)
            //{
            //    EnableSsl = true,
            //    Credentials = new NetworkCredential("MS_wWqgX8@trial-351ndgwev25gzqx8.mlsender.net", _configuration["MailSettings:Password"])
            //};

            //await client.SendMailAsync(
            //    new MailMessage(from: "MS_wWqgX8@trial-351ndgwev25gzqx8.mlsender.net",
            //                    to: email,
            //                    subject: subject,
            //                    body: htmlMessage));

        }
    }
}
