using Infrastructure.EmailService.Model;

namespace Infrastructure.EmailService.Interface
{
    public interface IEmailSender
    {
        Task<string> SendEmailAsync(Message message);
    }
}
