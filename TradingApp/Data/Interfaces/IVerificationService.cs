namespace TradingApp.Data.Interfaces {
    public interface IVerificationService {
        Task<string> GenerateAndSendVerificationCodeAsync(string email, string firstName);
        Task<bool> VerifyCodeAsync(string email, string code);
        Task<bool> IsCodeValidAsync(string email, string code);
    }
}


