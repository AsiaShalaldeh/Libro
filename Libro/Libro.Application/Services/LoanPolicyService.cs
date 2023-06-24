using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Libro.Application.Services
{
    public class LoanPolicyService : ILoanPolicyService
    {
        private readonly IConfiguration _configuration;

        public LoanPolicyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoanPolicy GetLoanPolicy()
        {
            var loanPolicy = new LoanPolicy();
            _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
            return loanPolicy;
        }
        public int GetLoanDuration()
        {
            var loanPolicy = new LoanPolicy();
            _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
            return loanPolicy.LoanDurationInDays;
        }
        public decimal GetBorrowingFeePerDay()
        {
            var loanPolicy = new LoanPolicy();
            _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
            return loanPolicy.BorrowingFeePerDay;
        }
        public decimal GetLateFeePerDay()
        {
            var loanPolicy = new LoanPolicy();
            _configuration.GetSection("LoanPolicy").Bind(loanPolicy);
            return loanPolicy.LateFeePerDay;
        }
    }
}
