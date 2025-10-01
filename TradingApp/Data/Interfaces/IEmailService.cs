namespace TradingApp.Data.Interfaces {
    public interface IEmailService {
        Task<bool> SendVerificationCodeAsync(string email, string verificationCode, string firstName);
        Task<bool> SendWelcomeEmailAsync(string email, string firstName);
    }
}


