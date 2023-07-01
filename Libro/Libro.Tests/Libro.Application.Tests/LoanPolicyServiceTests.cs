using Libro.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Application.Tests
{
    public class LoanPolicyServiceTests
    {
        private readonly Mock<ILogger<LoanPolicyService>> _loggerMock;
        private readonly LoanPolicyService _loanPolicyService;

        public LoanPolicyServiceTests()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/appsettings.json");

            _loggerMock = new Mock<ILogger<LoanPolicyService>>();
            _loanPolicyService = new LoanPolicyService(
                configBuilder.Build(),
                _loggerMock.Object
            );
        }

        [Fact]
        public void GetLoanPolicy_ReturnsLoanPolicyFromConfiguration()
        {
            // Arrange
            // Done Previously

            // Act
            var result = _loanPolicyService.GetLoanPolicy();

            // Assert
            Assert.Equal(14, result.LoanDurationInDays);
            Assert.Equal(2.0m, result.BorrowingFeePerDay);
            Assert.Equal(1.0m, result.LateFeePerDay);
            Assert.Equal(5, result.MaxBooksPerPatron);
        }
        [Fact]
        public void GetLoanDuration_ReturnsLoanDurationFromConfiguration()
        {
            // Act
            var result = _loanPolicyService.GetLoanDuration();

            // Assert
            Assert.Equal(14, result);
        }

        [Fact]
        public void GetBorrowingFeePerDay_ReturnsBorrowingFeePerDayFromConfiguration()
        {
            // Act
            var result = _loanPolicyService.GetBorrowingFeePerDay();

            // Assert
            Assert.Equal(2.0m, result);
        }

        [Fact]
        public void GetLateFeePerDay_ReturnsLateFeePerDayFromConfiguration()
        {

            // Act
            var result = _loanPolicyService.GetLateFeePerDay();

            // Assert
            Assert.Equal(1.0m, result);
        }
    }
}
