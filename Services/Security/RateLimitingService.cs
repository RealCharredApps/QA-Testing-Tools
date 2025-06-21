using NUnit.Framework;
using QaMastery.Models.Results;
using QaMastery.Services.Security;

namespace QaMastery.Services.Security
{
    public class RateLimitingService
    {
        private Dictionary<string, LoginAttemptInfo> _attempts = new Dictionary<string, LoginAttemptInfo>();
        private readonly int _maxAttempts = 5;
        private readonly TimeSpan _blockDuration = TimeSpan.FromMinutes(15);

        public bool IsAllowed(string username)
        {
            if (!_attempts.ContainsKey(username))
            {
                Console.WriteLine($"📊 {username}: First attempt, allowing");
                return true;
            }

            var attemptInfo = _attempts[username];

            // Check if user is currently blocked
            if (attemptInfo.BlockedUntil.HasValue && DateTime.UtcNow < attemptInfo.BlockedUntil.Value)
            {
                Console.WriteLine($"🛡️ {username}: Blocked until {attemptInfo.BlockedUntil.Value:HH:mm:ss}");
                return false;
            }

            // Check if user has exceeded max attempts
            if (attemptInfo.FailedAttempts >= _maxAttempts)
            {
                Console.WriteLine($"🛡️ {username}: Exceeded max attempts ({attemptInfo.FailedAttempts}/{_maxAttempts})");
                return false;
            }

            Console.WriteLine($"📊 {username}: {attemptInfo.FailedAttempts}/{_maxAttempts} attempts, allowing");
            return true;
        }

        public void RecordFailedAttempt(string username)
        {
            if (!_attempts.ContainsKey(username))
            {
                _attempts[username] = new LoginAttemptInfo();
            }

            var attemptInfo = _attempts[username];
            attemptInfo.FailedAttempts++;
            attemptInfo.LastAttempt = DateTime.UtcNow;

            if (attemptInfo.FailedAttempts >= _maxAttempts)
            {
                attemptInfo.BlockedUntil = DateTime.UtcNow.Add(_blockDuration);
                Console.WriteLine($"🚨 {username}: BLOCKED after {attemptInfo.FailedAttempts} failed attempts");
            }
            else
            {
                Console.WriteLine($"📈 {username}: Failed attempt #{attemptInfo.FailedAttempts}");
            }
        }

        public void RecordSuccessfulAttempt(string username)
        {
            if (_attempts.ContainsKey(username))
            {
                _attempts[username].FailedAttempts = 0;
                _attempts[username].BlockedUntil = null;
                Console.WriteLine($"✅ {username}: Successful login, reset counters");
            }
        }

        public void ResetCooldownForTesting(string username)
        {
            if (_attempts.ContainsKey(username))
            {
                _attempts[username].FailedAttempts = 0;
                _attempts[username].BlockedUntil = null;
                Console.WriteLine($"🔧 {username}: Rate limiting reset for testing");
            }
        }
    }

    public class LoginAttemptInfo
    {
        public int FailedAttempts { get; set; }
        public DateTime LastAttempt { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }


}