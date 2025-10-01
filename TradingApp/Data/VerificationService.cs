using TradingApp.Data.Interfaces;

namespace TradingApp.Data {
    public class VerificationService : IVerificationService {
        private readonly IEmailService _emailService;
        private readonly ILogger<VerificationService> _logger;
        
        // In-memory storage for demo purposes
        // In production, you'd store this in a database or Redis
        private static readonly Dictionary<string, (string Code, DateTime Expiry)> _verificationCodes = new();

        public VerificationService(IEmailService emailService, ILogger<VerificationService> logger) {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<string> GenerateAndSendVerificationCodeAsync(string email, string firstName) {
            try {
                // Generate 6-digit code
                var random = new Random();
                var code = random.Next(100000, 999999).ToString();
                
                // Set expiry time (10 minutes from now)
                var expiry = DateTime.UtcNow.AddMinutes(10);
                
                // Store the code
                _verificationCodes[email.ToLower()] = (code, expiry);
                
                // Send email
                var emailSent = await _emailService.SendVerificationCodeAsync(email, code, firstName);
                
                if (!emailSent) {
                    _logger.LogError($"Failed to send verification email to {email}");
                    return string.Empty;
                }
                
                _logger.LogInformation($"Verification code sent to {email}: {code}");
                return code;
            } catch (Exception ex) {
                _logger.LogError(ex, $"Error generating verification code for {email}");
                return string.Empty;
            }
        }

        public async Task<bool> VerifyCodeAsync(string email, string code) {
            try {
                var emailKey = email.ToLower();
                
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Verifying code for {emailKey}");
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Code provided: {code}");
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Total codes in storage: {_verificationCodes.Count}");
                
                if (!_verificationCodes.ContainsKey(emailKey)) {
                    Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: No code found for email {emailKey}");
                    return false;
                }
                
                var (storedCode, expiry) = _verificationCodes[emailKey];
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Stored code: {storedCode}");
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Expiry: {expiry}");
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Current time: {DateTime.UtcNow}");
                
                // Check if code has expired
                if (DateTime.UtcNow > expiry) {
                    Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Code has expired");
                    _verificationCodes.Remove(emailKey);
                    return false;
                }
                
                // Check if code matches
                if (storedCode != code) {
                    Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Code doesn't match - stored: '{storedCode}', provided: '{code}'");
                    return false;
                }
                
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Code verification successful!");
                // Code is valid - remove it to prevent reuse
                _verificationCodes.Remove(emailKey);
                return true;
            } catch (Exception ex) {
                Console.WriteLine($"🔍 VERIFICATION SERVICE DEBUG: Exception: {ex.Message}");
                _logger.LogError(ex, $"Error verifying code for {email}");
                return false;
            }
        }

        public async Task<bool> IsCodeValidAsync(string email, string code) {
            try {
                var emailKey = email.ToLower();
                
                if (!_verificationCodes.ContainsKey(emailKey)) {
                    return false;
                }
                
                var (storedCode, expiry) = _verificationCodes[emailKey];
                
                // Check if code has expired
                if (DateTime.UtcNow > expiry) {
                    return false;
                }
                
                // Check if code matches
                return storedCode == code;
            } catch (Exception ex) {
                _logger.LogError(ex, $"Error checking code validity for {email}");
                return false;
            }
        }
    }
}


