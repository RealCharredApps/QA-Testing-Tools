// ValidationResult.cs

namespace QaMastery.Models.Results
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string SanitizedInput { get; set; } = "";

        internal bool ErrorMessages()
        {
            throw new NotImplementedException();
        }
    }
}