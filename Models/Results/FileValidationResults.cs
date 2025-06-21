namespace QaMastery.Models.Results
{
    public class FileValidationResult
    {
        public bool IsBlocked { get; set; }
        public string Reason { get; set; } = "";
        public List<string> Violations { get; set; } = new List<string>();
    }
}