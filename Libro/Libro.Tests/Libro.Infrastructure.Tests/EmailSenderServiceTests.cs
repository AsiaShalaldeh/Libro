using EmailService.Service;
using EmailService.Interface;
using EmailService.Model;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class EmailSenderServiceTests
    {
        private readonly Mock<ILogger<EmailSenderService>> _loggerMock;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public EmailSenderServiceTests()
        {
            _loggerMock = new Mock<ILogger<EmailSenderService>>();
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var emailConfig = _configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            _emailSender = new EmailSenderService(emailConfig, _loggerMock.Object);
        }

        [Fact]
        public async Task SendEmailAsync_Success()
        {
            // Arrange
            var toAddresses = new List<string> { "recipient@example.com" };
            var message = new Message(toAddresses, "Test Subject", "Test Content");

            // Act
            var result = await _emailSender.SendEmailAsync(message);

            // Assert
            Assert.Equal("Email Sent Successfully", result);
        }
    }
}
