using System.Net;
using System.Net.Mail;
using TradingApp.Data.Interfaces;

namespace TradingApp.Data {
    public class EmailService : IEmailService {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger) {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendVerificationCodeAsync(string email, string verificationCode, string firstName) {
            try {
                // For development/demo purposes, we'll log the email instead of actually sending it
                // In production, you would configure SMTP settings
                _logger.LogInformation($"Verification Code for {email}: {verificationCode}");
                
                // Simulate email sending delay
                await Task.Delay(1000);
                
                return true;
            } catch (Exception ex) {
                _logger.LogError(ex, $"Failed to send verification code to {email}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string firstName) {
            try {
                _logger.LogInformation($"Welcome email sent to {email} for user {firstName}");
                
                // Simulate email sending delay
                await Task.Delay(500);
                
                return true;
            } catch (Exception ex) {
                _logger.LogError(ex, $"Failed to send welcome email to {email}");
                return false;
            }
        }
    }
}


