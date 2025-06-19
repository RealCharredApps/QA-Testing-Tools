// AttackSimulator.cs
using QaMastery.Models.Results;

namespace QaMastery.Services.Security
{
    public class AttackSimulator
    {
        public AuthenticationResult AttemptLogin(string username, string password)
        {
            // TODO: Simulate authentication that detects bypass attempts
            var securityFlags = new List<string>();

            if (username?.Contains("'") == true) securityFlags.Add("SQL_INJECTION_ATTEMPT");
            if (username?.Contains("../") == true) securityFlags.Add("PATH_TRAVERSAL_ATTEMPT");
            if (username?.Contains("\x00") == true) securityFlags.Add("NULL_BYTE_INJECTION");

            return new AuthenticationResult
            {
                IsSuccessful = false,
                IsBlocked = securityFlags.Count > 0,
                SecurityFlags = securityFlags,
                Message = securityFlags.Count > 0
                    ? $"Blocked due to: {string.Join(", ", securityFlags)}"
                    : "Authentication failed"
            };
        }
    }
}