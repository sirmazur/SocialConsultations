using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;

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
            emailToSend.From.Add(MailboxAddress.Parse("DialogBridge@outlook.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.Connect("smtp-mail.outlook.com", 587, 
                    MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("DialogBridge@outlook.com", _configuration["MailSettings:Password"]);
                await emailClient.SendAsync(emailToSend);
            }
        }
    }
}
