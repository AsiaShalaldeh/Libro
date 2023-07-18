using EmailService.Model;

namespace EmailService.Interface
{
    public interface IEmailSender
    {
        Task<string> SendEmailAsync(Message message);
    }
}
