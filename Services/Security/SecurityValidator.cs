// SecurityValidator.cs
using System;
using System.Text.RegularExpressions;
using QaMastery.Models.Results;
using QaMastery.Models.Enums;

namespace QaMastery.Services.Security
{
    public class SecurityValidator
    {
        public ValidationResult ValidateEmail(string email)
        {
            // Implement comprehensive email validation
            // Consider: format, length, special characters, injection attempts
            var result = new ValidationResult();
            //SEcurity Check 1 email
            if (string.IsNullOrWhiteSpace(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Email cannot be empty";
                result.SanitizedInput = "";
                return result;
            }
            //Security Check 2 sql
            if (ContainsSqlInjectionPatterns(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid characters detected";
                result.SanitizedInput = SanitizeInput(email);
                return result;
            }
            //Security Check 3 xss
            if (ContainsXssPatterns(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid HTML/script content detected";
                result.SanitizedInput = SanitizeInput(email);
                return result;
            }
            //Security Check 4 email header injection
            if (ContainsEmailHeaderInjection(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid email format - header injection detected";
                result.SanitizedInput = SanitizeInput(email);
                return result;
            }
            // SECURITY CHECK 5: Detect path traversal attempts
            if (ContainsPathTraversal(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid characters in email address";
                result.SanitizedInput = SanitizeInput(email);
                return result;
            }
            // BUSINESS LOGIC VALIDATION: Basic email format
            if (!IsValidEmailFormat(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid email format";
                result.SanitizedInput = email; // No sanitization needed for format issues
                return result;
            }

            // SUCCESS: Email passed all security and format checks
            result.IsValid = true;
            result.ErrorMessage = "";
            result.SanitizedInput = email;
            return result;
        }

        private bool IsValidEmailFormat(string email)
        {
            // Basic email format validation (not security-focused)
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Must contain exactly one @ symbol
            var atCount = email.Split('@').Length - 1;
            if (atCount != 1) return false;

            var parts = email.Split('@');
            var localPart = parts[0];
            var domainPart = parts[1];

            // Local part (before @) cannot be empty
            if (string.IsNullOrWhiteSpace(localPart)) return false;

            // Domain part (after @) cannot be empty and must contain a dot
            if (string.IsNullOrWhiteSpace(domainPart) || !domainPart.Contains(".")) return false;

            // Basic length checks
            if (email.Length > 254) return false; // RFC limit
            if (localPart.Length > 64) return false; // RFC limit

            return true;
        }

        private bool ContainsPathTraversal(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            return email.Contains("../") || email.Contains("..\\") ||
                   email.Contains("/etc/") || email.Contains("\\windows\\");
        }

        private bool ContainsEmailHeaderInjection(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            // Email header injection uses newlines to inject additional headers
            return email.Contains("\r") || email.Contains("\n") ||
                   email.Contains("BCC:") || email.Contains("CC:") ||
                   email.Contains("TO:") || email.Contains("FROM:");
        }

        private bool ContainsXssPatterns(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var xssPatterns = new[]
            {
                "<script",
                "</script>",
                "javascript:",
                "vbscript:",
                "onload=",
                "onerror=",
                "onclick=",
                "onmouseover=",
                "<iframe",
                "<object",
                "<embed"
            };

            var lowerInput = email.ToLowerInvariant();
            foreach (var pattern in xssPatterns)
            {
                if (lowerInput.Contains(pattern))
                {
                    return true;
                }
            }
            return false;
        }

        private string SanitizeInput(string email)
        {
            if (string.IsNullOrEmpty(email)) return "";

            // Remove dangerous characters
            return email
                .Replace("<", "")
                .Replace(">", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(";", "")
                .Replace("--", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("../", "");
        }

        private bool ContainsSqlInjectionPatterns(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var sqlPatterns = new[]
            {
                "DROP TABLE",
                "DELETE FROM",
                "INSERT INTO",
                "UPDATE SET",
                "UNION SELECT",
                "OR '1'='1'",
                "'", // Single quotes are suspicious in email context
                "--", // SQL comments
                "/*", "*/" // SQL block comments
            };

            var upperInput = email.ToUpperInvariant();
            foreach (var pattern in sqlPatterns)
            {
                if (upperInput.Contains(pattern.ToUpperInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        public ValidationResult ValidatePassword(string password, string username = null)
        {
            // TODO: Implement password validation with security focus
            // - Minimum length (8+ characters)
            // - Complexity requirements (upper, lower, number, special)
            // - Common password detection
            // - Username similarity check
            return new ValidationResult { IsValid = password?.Length >= 8 };
        }

        public SanitizationResult SanitizeUserInput(string input)
        {
            // TODO: Implement input sanitization that blocks SQL injection
            var containsSqlKeywords = input?.ToLower().Contains("drop table") == true ||
                                    input?.ToLower().Contains("union select") == true ||
                                    input?.Contains("'") == true;

            return new SanitizationResult
            {
                IsBlocked = containsSqlKeywords,
                SanitizedInput = input?.Replace("'", "''")!,
                ThreatLevel = containsSqlKeywords ? ThreatLevel.High : ThreatLevel.None
            };
        }

        public SanitizationResult SanitizeHtmlInput(string input)
        {
            // TODO: Implement HTML sanitization that prevents XSS
            var sanitized = input?
                .Replace("<script>", "")
                .Replace("</script>", "")
                .Replace("javascript:", "")
                .Replace("onerror=", "")
                .Replace("onload=", "");

            return new SanitizationResult
            {
                SanitizedInput = sanitized!,
                ThreatLevel = input != sanitized ? ThreatLevel.Medium : ThreatLevel.None
            };
        }
    }
}