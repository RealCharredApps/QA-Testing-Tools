using NUnit.Framework;
using QaMastery.Services.Core;
using QaMastery.Services.Security;
using QaMastery.Models.Results;
using QaMastery.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using QaMastery.Models.TestCases;

namespace QaMastery.Week1.SecurityTesting
{
    // ============================================================================
    // DAY 3 FOCUS: Security Input Validation with TestCase Patterns
    // ============================================================================

    [TestFixture]
    [Category("Security")]
    [Category("InputValidation")]
    public class SecurityInputValidationTests
    {
        private SecurityValidator _validator = null!;
        private List<string> _testResults = null!;
        private AttackSimulator _attackSimulator = null!;
        private RateLimitingService _rateLimitingService = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new SecurityValidator();
            _testResults = new List<string>();
            _attackSimulator = new AttackSimulator();
            _rateLimitingService = new RateLimitingService();
        }

        // EXERCISE 1: Email validation with security focus
        // Super Security needs rock-solid email validation (it's their primary identifier)

        [TestCase("valid@Super Security.com", true, TestName = "Valid email should pass")]
        [TestCase("user+tag@company.co.uk", true, TestName = "Plus addressing should work")]
        [TestCase("", false, TestName = "Empty email should fail")] //✅FIXED: FAILED THROUGH
        [TestCase("notanemail", false, TestName = "Missing @ should fail")] //✅FIXED business logic: FAILED THROUGH
        [TestCase("test@", false, TestName = "Missing domain should fail")] //✅FIXED business logic: FAILED THROUGH
        [TestCase("@domain.com", false, TestName = "Missing username should fail")] //✅FIXED business logic: FAILED THROUGH
        // SECURITY-FOCUSED test cases (what attackers try):
        [TestCase("test'; DROP TABLE users; --@evil.com", false, TestName = "SQL injection in email should fail")] //✅FIXED: FAILED THROUGH
        [TestCase("test<script>alert('xss')</script>@domain.com", false, TestName = "XSS in email should fail")] //✅FIXED in sql/san...: FAILED THROUGH
        [TestCase("test@domain.com\r\nBCC: attacker@evil.com", false, TestName = "Email header injection should fail")] //✅FIXED: FAILED THROUGH
        [TestCase("../../../etc/passwd@domain.com", false, TestName = "Path traversal attempt should fail")] //✅FIXED: FAILED THROUGH
        public void ValidateEmail_SecurityScenarios_ShouldPreventAttacks(string email, bool expectedValid)
        {
            // ACT
            var result = _validator.ValidateEmail(email);

            // ASSERT
            Assert.That(result.IsValid, Is.EqualTo(expectedValid));

            // SECURITY LOGGING: Track what we tested
            _testResults.Add($"{(result.IsValid == expectedValid ? "✅" : "❌")}");

            // If input should be invalid, ensure it's properly blocked
            if (!expectedValid)
            {
                Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty,
                           "Invalid input should have error message");
                Assert.That(result.SanitizedInput, Does.Not.Contain("<script>"),
                           "Sanitized input should not contain script tags");
            }
        }







        // EXERCISE 2: Password security validation
        // Super Security stores the most sensitive data - passwords must be bulletproof

        [TestCase("Password123!", true, TestName = "Strong password should pass")]
        [TestCase("MySecureP@ssw0rd", true, TestName = "Complex password should pass")]
        [TestCase("password", false, TestName = "Weak password should fail")] //FAILED THROUGH
        [TestCase("12345678", false, TestName = "Numbers only should fail")] //FAILED THROUGH
        [TestCase("PASSWORD", false, TestName = "Uppercase only should fail")] //FAILED THROUGH
        [TestCase("pass", false, TestName = "Too short should fail")] //FAILED THROUGH
        [TestCase("", false, TestName = "Empty password should fail")] //FAILED THROUGH
        // SECURITY EDGE CASES:
        [TestCase("john@example.com", false, TestName = "Email as password should fail")] //FAILED THROUGH
        [TestCase("admin", false, TestName = "Common admin password should fail")] //FAILED THROUGH
        [TestCase("qwerty123", false, TestName = "Keyboard pattern should fail")] //FAILED THROUGH
        [TestCase("Password123!Password123!", false, TestName = "Overly long password should be handled")] //FAILED THROUGH
        public void ValidatePassword_SecurityRequirements_ShouldEnforcePolicy(string password, bool expectedValid)
        {
            // ARRANGE
            string username = "testuser@SuperSecurity.com";

            // ACT
            var result = _validator.ValidatePassword(password, username);

            // ASSERT
            Assert.That(result.IsValid, Is.EqualTo(expectedValid));

            if (!expectedValid)
            {
                Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty,
                           "Failed validation should provide a specific error message");
                Console.WriteLine($"Password '{password}' failed: {result.ErrorMessage}");
            }
        }

        [TearDown]
        public void LogSecurityTestResults()
        {
            Console.WriteLine("=== Security Test Results ===");
            foreach (var result in _testResults)
            {
                Console.WriteLine(result);
            }
        }
    }

    // ============================================================================
    // DAY 4 FOCUS: Advanced Security Testing with TestCaseSource
    // ============================================================================

    [TestFixture]
    [Category("Security")]
    [Category("AdvancedAttacks")]
    public class AdvancedSecurityTests
    {
        private SecurityValidator _validator = null!;
        private AttackSimulator _attackSimulator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new SecurityValidator();
            _attackSimulator = new AttackSimulator();
        }

        // EXERCISE 3: SQL Injection testing with comprehensive payloads
        // Use TestCaseSource for complex attack scenarios

        [TestCaseSource(nameof(SqlInjectionPayloads))]
        public void UserInput_SqlInjectionPayloads_ShouldBeBlocked(SqlInjectionTestCase testCase)
        {
            Console.WriteLine($"Testing SQL injection: {testCase.Description}");

            // ACT
            var result = _validator.SanitizeUserInput(testCase.Payload);

            // ASSERT
            Assert.Multiple(() =>
            {
                Assert.That(result.IsBlocked, Is.True,
                           $"SQL injection payload should be blocked: {testCase.Description}");
                Assert.That(result.SanitizedInput, Does.Not.Contain("DROP TABLE"),
                           "Sanitized input should not contain dangerous SQL");
                Assert.That(result.SanitizedInput, Does.Not.Contain("UNION SELECT"),
                           "Sanitized input should not contain union statements");
                Assert.That(result.ThreatLevel, Is.EqualTo(testCase.ExpectedThreatLevel),
                           "Threat level should match expected severity");
            });

            // Log security event (like Super Security would)
            LogSecurityEvent($"SQL_INJECTION_BLOCKED", testCase.Payload, result.ThreatLevel);
        }

        // EXERCISE 4: Cross-Site Scripting (XSS) prevention
        [TestCaseSource(nameof(XssPayloads))]
        public void UserInput_XssPayloads_ShouldBeSanitized(XssTestCase testCase)
        {
            Console.WriteLine($"Testing XSS prevention: {testCase.Description}");

            // ACT
            var result = _validator.SanitizeHtmlInput(testCase.Payload);

            // ASSERT
            Assert.Multiple(() =>
            {
                Assert.That(result.SanitizedInput, Does.Not.Contain("<script>"),
                           "Should remove script tags");
                Assert.That(result.SanitizedInput, Does.Not.Contain("javascript:"),
                           "Should remove javascript protocols");
                Assert.That(result.SanitizedInput, Does.Not.Contain("onerror="),
                           "Should remove event handlers");
                Assert.That(result.SanitizedInput, Does.Not.Contain("onload="),
                           "Should remove onload handlers");
            });

            LogSecurityEvent($"XSS_SANITIZED", testCase.Payload, testCase.ExpectedThreatLevel);
        }

        // EXERCISE 5: Authentication bypass attempts
        [TestCaseSource(nameof(AuthBypassPayloads))]
        public void Authentication_BypassAttempts_ShouldBePrevented(AuthTestCase testCase)
        {
            Console.WriteLine($"Testing auth bypass: {testCase.Description}");

            // ACT
            var authResult = _attackSimulator.AttemptLogin(testCase.Username, testCase.Password);

            // ASSERT
            Assert.Multiple(() =>
            {
                Assert.That(authResult.IsSuccessful, Is.False,
                           "Authentication bypass should fail");
                Assert.That(authResult.IsBlocked, Is.True,
                           "Malicious attempts should be blocked");
                Assert.That(authResult.SecurityFlags.Count, Is.GreaterThan(0),
                           "Security violations should be flagged");
            });
        }

        // ========================================================================
        // TEST DATA SOURCES - Real attack patterns that Super Security defends against
        // ========================================================================

        public static IEnumerable<SqlInjectionTestCase> SqlInjectionPayloads()
        {
            yield return new SqlInjectionTestCase
            {
                Payload = "'; DROP TABLE users; --",
                Description = "Table deletion attempt",
                ExpectedThreatLevel = ThreatLevel.Critical
            };

            yield return new SqlInjectionTestCase
            {
                Payload = "' OR '1'='1",
                Description = "Always true condition",
                ExpectedThreatLevel = ThreatLevel.High
            };

            yield return new SqlInjectionTestCase
            {
                Payload = "' UNION SELECT password FROM users WHERE email='admin@company.com' --",
                Description = "Data extraction attempt",
                ExpectedThreatLevel = ThreatLevel.Critical
            };

            yield return new SqlInjectionTestCase
            {
                Payload = "'; INSERT INTO users (email, role) VALUES ('hacker@evil.com', 'admin'); --",
                Description = "Privilege escalation attempt",
                ExpectedThreatLevel = ThreatLevel.Critical
            };

            yield return new SqlInjectionTestCase
            {
                Payload = "admin'/*",
                Description = "Comment-based injection",
                ExpectedThreatLevel = ThreatLevel.Medium
            };

            // TODO: Add 5 more SQL injection patterns you can think of
        }

        public static IEnumerable<XssTestCase> XssPayloads()
        {
            yield return new XssTestCase
            {
                Payload = "<script>alert('XSS')</script>",
                Description = "Basic script injection",
                ExpectedThreatLevel = ThreatLevel.High
            };

            yield return new XssTestCase
            {
                Payload = "<img src=x onerror=alert('XSS')>",
                Description = "Image tag with error handler",
                ExpectedThreatLevel = ThreatLevel.High
            };

            yield return new XssTestCase
            {
                Payload = "javascript:alert('XSS')",
                Description = "JavaScript protocol",
                ExpectedThreatLevel = ThreatLevel.Medium
            };

            yield return new XssTestCase
            {
                Payload = "<svg onload=alert('XSS')>",
                Description = "SVG with onload event",
                ExpectedThreatLevel = ThreatLevel.High
            };

            yield return new XssTestCase
            {
                Payload = "<iframe src=\"javascript:alert('XSS')\"></iframe>",
                Description = "iframe with javascript source",
                ExpectedThreatLevel = ThreatLevel.High
            };

            // TODO: Add more XSS payloads
        }

        public static IEnumerable<AuthTestCase> AuthBypassPayloads()
        {
            yield return new AuthTestCase
            {
                Username = "admin' OR '1'='1' --",
                Password = "anything",
                Description = "SQL injection in username field"
            };

            yield return new AuthTestCase
            {
                Username = "admin",
                Password = "' OR '1'='1' --",
                Description = "SQL injection in password field"
            };

            yield return new AuthTestCase
            {
                Username = "../../../etc/passwd",
                Password = "password",
                Description = "Path traversal in username"
            };

            yield return new AuthTestCase
            {
                Username = "admin\x00",
                Password = "password",
                Description = "Null byte injection"
            };

            // TODO: Add more authentication bypass attempts
        }

        private void LogSecurityEvent(string eventType, string payload, ThreatLevel threatLevel)
        {
            Console.WriteLine($"[SECURITY LOG] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {eventType} - {threatLevel} - Payload: {payload.Substring(0, Math.Min(50, payload.Length))}...");
        }
    }

    // ============================================================================
    // YOUR CHALLENGE EXERCISES
    // ============================================================================

    [TestFixture]
    [Category("YourSecurityPractice")]
    public class YourSecurityTestChallenge
    {
        // CHALLENGE 1: Create a comprehensive file upload security test
        private FileUploadService _fFileUploadService = null!;
        private AttackSimulator _attackSimulator = null!;
        private RateLimitingService _rateLimitingService = null!;
        private Dictionary<string, AttemptInfo> _attempts = new Dictionary<string, AttemptInfo>();

        [Test]
        public void FileUpload_MaliciousFiles_ShouldBeBlocked()
        {
            _fFileUploadService = new FileUploadService();
            // Test various malicious file types
            var maliciousFiles = new TestFile[]
            {
                // - Executable files (.exe, .bat, .sh)
                new TestFile
                {
                    FileName = "virus.exe",
                    Content = GetExecutableBytes(),
                    ShouldBeBlocked = true,
                    AttackType = "Executable Upload"
                },
                // - Script files (.php, .js, .py)
                new TestFile
                {
                    FileName = "shell.php",
                    Content = GetPhpShellBytes(),
                    ShouldBeBlocked = true,
                    AttackType = "Server Script Upload"
                },
                // LEGITIMATE FILES (should be allowed)
                new TestFile
                {
                    FileName = "photo.jpg",
                    Content = GetJpegBytes(),
                    ShouldBeBlocked = false,
                    AttackType = "Legitimate Image"
                },
                new TestFile
                {
                    FileName = "document.pdf",
                    Content = GetPdfBytes(),
                    ShouldBeBlocked = false,
                    AttackType = "Legitimate Document"
                }
                // - Files with double extensions (.jpg.exe)
                // - Files with null bytes in filename
                // - Extremely large files (DoS attempt)
                // - Files with path traversal in filename (../../etc/passwd)
            };
            foreach (var testFile in maliciousFiles)
            {
                Console.WriteLine($"🔍 Testing: {testFile.FileName} ({testFile.AttackType})");
                var result = _fFileUploadService.ValidateFile(testFile);
                Console.WriteLine($"📊 Result: IsBlocked={result.IsBlocked}, Reason='{result.Reason}'");

                if (testFile.ShouldBeBlocked)
                {
                    Assert.That(result.IsBlocked, Is.True,
                                $"{testFile.AttackType} - Expected blocked: {testFile.ShouldBeBlocked}, Result: {result.Reason}");
                    Console.WriteLine($"✅ {testFile.AttackType} correctly BLOCKED");
                }
                else
                {
                    Assert.That(result.IsBlocked, Is.False,
                               $"{testFile.AttackType} should be ALLOWED but was BLOCKED! Reason: {result.Reason}");
                    Console.WriteLine($"✅ {testFile.AttackType} correctly ALLOWED");
                }

                Console.WriteLine("---");
            }
            Console.WriteLine("🎯 All file upload security tests completed successfully!");
            //Assert.Fail("Implement comprehensive file upload security testing");
        }

        private byte[] GetPhpShellBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes("<?php system($_GET['cmd']); ?>");
        }

        private byte[] GetExecutableBytes()
        {
            return new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00 };
        }

        private byte[] GetJpegBytes()
        {
            // JPEG file signature (magic bytes)
            return new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header
        }

        private byte[] GetPdfBytes()
        {
            // PDF file signature
            return System.Text.Encoding.UTF8.GetBytes("%PDF-1.4");
        }



        // CHALLENGE 2: Create a rate limiting test
        [Test]
        public void Authentication_RapidAttempts_ShouldBeRateLimited()
        {
            // Test that rapid authentication attempts are blocked
            // Simulate what an attacker would do:
            // - 10 login attempts in 1 second
            // - Verify rate limiting kicks in
            // - Verify legitimate users can still login after cooldown
            _rateLimitingService = new RateLimitingService();
            var authService = new AuthService();
            var testUser = "testuser@example.com";
            var wrongPassword = "wrongpassword";
            var correctPassword = "correctpassword";

            Console.WriteLine("🔍 Testing rate limiting for rapid login attempts");

            for (int attempt = 1; attempt <= 7; attempt++)
            {
                Console.WriteLine($"🚀 Attempt {attempt}: Trying wrong password");

                var result = authService.Login(testUser, wrongPassword);
                // ACT & ASSERT - Part 1: Rapid failures should get blocked
                if (attempt <= 5)
                {
                    // First 5 attempts should be processed (but fail due to wrong password)
                    Assert.That(result.IsBlocked, Is.False, $"Attempt {attempt} should be processed, not rate limited");
                    Assert.That(result.IsSuccess, Is.False, $"Attempt {attempt} should fail due to wrong password");
                    Console.WriteLine($"📊 Attempt {attempt}: Processed but failed (as expected)");
                }
                else
                {
                    // Attempts 6+ should be rate limited
                    Assert.That(result.IsBlocked, Is.True, $"Attempt {attempt} should be rate limited");
                    Console.WriteLine($"🛡️ Attempt {attempt}: RATE LIMITED (success!)");
                }
            }
            // ACT & ASSERT - Part 2: Even correct password should be blocked during rate limit
            Console.WriteLine("🔍 Testing that correct password is blocked during rate limit");
            var blockedResult = authService.Login(testUser, correctPassword);
            Assert.That(blockedResult.IsBlocked, Is.True, "Even correct password should be blocked during rate limit");
            Console.WriteLine($"🛡️ Correct password blocked during rate limit period (security working!)");

            // ACT & ASSERT - Part 3: After cooldown, legitimate login should work
            Console.WriteLine("🕐 Simulating cooldown period (waiting...)");
            // In real test, you'd either wait or mock time
            SimulateCooldownPeriod(); // Helper method to advance time

            var afterCooldownResult = authService.Login(testUser, correctPassword);
            Assert.That(afterCooldownResult.IsBlocked, Is.False, "Login should be allowed after cooldown");
            Assert.That(afterCooldownResult.IsSuccess, Is.True, "Correct password should work after cooldown");
            Console.WriteLine($"✅ Login successful after cooldown period");

            Console.WriteLine("🎯 Rate limiting test completed successfully!");

            //Assert.Fail("Implement rate limiting security test");
        }

        private void SimulateCooldownPeriod()
        {
            // Option 1: Actually wait (slow but realistic)
            // Thread.Sleep(TimeSpan.FromMinutes(1));

            // Option 2: Mock time (faster testing)
            // Advance the internal clock in your rate limiter

            // Option 3: Reset rate limiter manually for testing
            _rateLimitingService.ResetCooldownForTesting("testuser@example.com");
        }

        private void LogAttemptDetails(int attemptNumber, LoginResult result)
        {
            Console.WriteLine($"🔍 Attempt {attemptNumber}:");
            Console.WriteLine($"   IsSuccess: {result.IsSuccess}");
            Console.WriteLine($"   IsBlocked: {result.IsBlocked}");
            Console.WriteLine($"   Reason: {result.Reason}");
            Console.WriteLine($"   Timestamp: {DateTime.Now:HH:mm:ss.fff}");
            Console.WriteLine("---");
        }

        public void ResetCooldownForTesting(string username)
        {
            if (_attempts.ContainsKey(username))
            {
                _attempts[username].FailedAttempts = 0;
                _attempts[username].BlockedUntil = null;
                Console.WriteLine($"🔧 Reset rate limiting for {username} (testing helper)");
            }
        }

        // Helper class for attempt tracking
        public class AttemptInfo
        {
            public int FailedAttempts { get; set; }
            public DateTime? BlockedUntil { get; set; }
        }





        // CHALLENGE 3: Create session security tests
        [Test]
        public void SessionManagement_SecurityRequirements_ShouldBeEnforced()
        {
            // TODO: Test session security like Super Security does:
            // - Session tokens should be unpredictable
            // - Sessions should expire after inactivity
            // - Sessions should be invalidated on logout
            // - Multiple sessions should be tracked
            // - Session fixation should be prevented

            Assert.Fail("Implement session security testing");
        }

        //helpers

    }
}

// ============================================================================
// SUCCESS METRICS FOR DAYS 3-4
// ============================================================================

/*
By the end of Day 4, you should be able to:

✅ TECHNICAL SKILLS:
- Write TestCase arrays that cover security scenarios
- Use TestCaseSource for complex attack simulation
- Implement input validation that blocks real attacks
- Create comprehensive test data for security testing

✅ SECURITY MINDSET:
- Think like an attacker (what would I try?)
- Understand common attack vectors (SQLi, XSS, auth bypass)
- Design tests that validate security controls
- Log security events for monitoring

✅ Super Security-READY SKILLS:
- Test password validation like a security company
- Validate input sanitization thoroughly
- Simulate real-world attack scenarios
- Document security testing results

TIME INVESTMENT: 3-4 hours total
DAY 3: Implement basic security test patterns (2 hours)
DAY 4: Add advanced attack scenarios (2 hours)

NEXT UP: Week 1 Days 5-7 will add Playwright for testing Super Security's Electron desktop app security
*/