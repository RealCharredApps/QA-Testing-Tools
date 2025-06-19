namespace QaMastery.Models.Results
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public bool IsSuccessful { get; internal set; }
        public bool IsBlocked { get; set; }
        public List<string> SecurityFlags { get; set; } = new List<string>();
    }
}