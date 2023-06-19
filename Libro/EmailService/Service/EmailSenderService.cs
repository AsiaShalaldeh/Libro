using EmailService.Model;
using EmailService.Interface;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net;

namespace EmailService.Service
{
    public class EmailSenderService : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSenderService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task<string> SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage);
            return "Email Sent Successfully";
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Libro Library", _emailConfig.SenderEmail));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.CheckCertificateRevocation = false;
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.SenderName, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log an error message or handle the exception accordingly.
                    throw new Exception("Failed to send email", ex);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

    }
}
