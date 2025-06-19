using QaMastery.Models.Enums;

namespace QaMastery.Models.TestCases
{
    public class XssTestCase
    {
        public string Payload { get; set; } = "";
        public string Description { get; set; } = "";
        public ThreatLevel ExpectedThreatLevel { get; set; }
        
        public override string ToString() => Description;
    }
}