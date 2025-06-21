using System;

namespace QaMastery.Models.Results
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public bool IsBlocked { get; set; }
        public object FailureReason { get; internal set; } = null!;
        public object Reason { get; internal set; } = null!;
        public List<string> SecurityFlags { get; set; } = new List<string>();
        public bool IsSuccessful { get; internal set; }
    }
}