using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class LoanPolicyService : ILoanPolicyService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoanPolicyService> _logger;

        public LoanPolicyService(IConfiguration configuration, ILogger<LoanPolicyService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public LoanPolicy GetLoanPolicy()
        {
            try
            {
                var loanPolicy = new LoanPolicy();
                _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
                return loanPolicy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the loan policy.");
                throw;
            }
        }

        public int GetLoanDuration()
        {
            try
            {
                var loanPolicy = new LoanPolicy();
                _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
                return loanPolicy.LoanDurationInDays;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the loan duration.");
                throw;
            }
        }

        public decimal GetBorrowingFeePerDay()
        {
            try
            {
                var loanPolicy = new LoanPolicy();
                _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
                return loanPolicy.BorrowingFeePerDay;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the borrowing fee per day.");
                throw;
            }
        }

        public decimal GetLateFeePerDay()
        {
            try
            {
                var loanPolicy = new LoanPolicy();
                _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
                return loanPolicy.LateFeePerDay;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the late fee per day.");
                throw;
            }
        }
        public decimal GetMaxBooksPerPatron()
        {
            try
            {
                var loanPolicy = new LoanPolicy();
                _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
                return loanPolicy.MaxBooksPerPatron;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the max books per patron.");
                throw;
            }
        }
        public bool CanPatronCheckoutBook(Patron patron)
        {
            // Check if the patron has reached the maximum number of books allowed to be checked out
            var checkoutBooks = patron.CheckedoutBooks.Where(ch => ch.IsReturned == false).ToList();
            if (checkoutBooks.Count > GetMaxBooksPerPatron())
            {
                return false;
            }
            return true;
        }
        public decimal CalculateTotalFees(int checkoutDay, int dueDay, int returnDay)
        {
            var days = returnDay - dueDay;
            decimal total = Math.Round(GetLoanDuration() * GetBorrowingFeePerDay(), 2);

            if (days == 0)
            {
                return total;
            }
            else if (days > 0)
            {
                return Math.Round(total + (GetLateFeePerDay() * days), 2);
            }
            else if (days < 0)
            {
                days = returnDay - checkoutDay;
                return Math.Round((days + 1) * GetBorrowingFeePerDay(), 2);
            }
            return 0;
        }
    }
}
