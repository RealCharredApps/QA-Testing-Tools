

namespace QaMastery.Services.Security
{
    public class FileValidationResult
    {
        public bool IsBlocked { get; set; } = true!;   // Was the file rejected?
        public string Reason { get; set; } = null!;      // Why was it blocked/allowed?
        public string ThreatLevel { get; set; } = null!;    // How dangerous is this file?
        public List<string> Violations { get; set; } = new List<string>(); // Multiple issues?
    }
}