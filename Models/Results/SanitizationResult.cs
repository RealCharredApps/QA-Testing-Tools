using QaMastery.Models.Enums;

namespace QaMastery.Models.Results
{
    public class SanitizationResult
    {
        public bool IsBlocked { get; set; }
        public string SanitizedInput { get; set; } = "";
        public ThreatLevel ThreatLevel { get; set; }
    }
}