using System;
using QaMastery.Models.Results;

namespace QaMastery.Services.Security
{
    public class AuthService
    {
        private int _attemptCount = 0;
        private readonly RateLimitingService _rateLimiter;
        public AuthService()
        {
            _rateLimiter = new RateLimitingService();
        }

        public LoginResult Login(string username, string password)
        {
            Console.WriteLine($"🔍 Login attempt for: {username}");

            // STEP 1: Check rate limiting FIRST
            if (!_rateLimiter.IsAllowed(username))
            {
                Console.WriteLine($"🛡️ Rate limited: {username}");
                return new LoginResult
                {
                    IsSuccessful = false,
                    IsBlocked = true,
                    Reason = "Rate limited - too many failed attempts"
                };
            }

            // STEP 2: Check credentials
            bool isValidPassword = ValidateCredentials(username, password);

            if (isValidPassword)
            {
                Console.WriteLine($"✅ Successful login: {username}");
                _rateLimiter.RecordSuccessfulAttempt(username);
                return new LoginResult
                {
                    IsSuccessful = true,
                    IsBlocked = false,
                    Reason = "Login successful"
                };
            }
            else
            {
                Console.WriteLine($"❌ Failed login: {username}");
                _rateLimiter.RecordFailedAttempt(username);
                return new LoginResult
                {
                    IsSuccessful = false,
                    IsBlocked = false,
                    Reason = "Invalid credentials"
                };
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // Simple validation for testing
            return password == "correctpassword";
        }
    }
}