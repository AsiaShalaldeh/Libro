﻿using Libro.Domain.Interfaces.IServices;
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
    }
}
