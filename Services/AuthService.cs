using System;

namespace QaMastery.Week1.Day1
{
    public class AuthService
    {
        private int _attemptCount = 0;

        public LoginResult Login(string email, string password)
        {
            _attemptCount++;
            return new LoginResult
            {
                IsSuccess = false,
                IsBlocked = _attemptCount >= 5
            };
        }
    }
}