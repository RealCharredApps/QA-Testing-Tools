using QaMastery.Models.Results;
using QaMastery.Models.TestCases;

namespace QaMastery.Services.Security
{
    public class FileUploadService
    {
        public FileValidationResult ValidateFile(TestFile file)
        {
            var result = new FileValidationResult();

            // Extension validation
            var dangerousExtensions = new[] { ".exe", ".bat", ".php", ".js", ".sh", ".py" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            Console.WriteLine($"📋 File extension: '{fileExtension}'");
            Console.WriteLine($"🚨 Is dangerous: {dangerousExtensions.Contains(fileExtension)}");

            if (dangerousExtensions.Contains(fileExtension))
            {
                result.IsBlocked = true;
                result.Reason = $"Dangerous file extension: {fileExtension}";
                Console.WriteLine($"🛡️ BLOCKING: {result.Reason}");
                return result;
            }

            // Path traversal detection  
            if (file.FileName.Contains("..") || file.FileName.Contains("/") || file.FileName.Contains("\\"))
            {
                result.IsBlocked = true;
                result.Reason = "Path traversal detected in filename";
                Console.WriteLine($"🛡️ BLOCKING: {result.Reason}");
                return result;
            }

            // File size validation (example: 10MB limit)
            if (file.Content.Length > 10 * 1024 * 1024)
            {
                result.IsBlocked = true;
                result.Reason = "File too large";
                return result;
            }

            // If we get here, file passed all checks
            result.IsBlocked = false;
            result.Reason = "File validation passed";
            Console.WriteLine($"✅ Allowing: {result.Reason}");
            return result;
        }
    }
}