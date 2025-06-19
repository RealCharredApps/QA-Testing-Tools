using System;

namespace QaMastery.Week1.Day1
{
    public class CreateUserResult
    {
        public bool IsSuccess { get; set; }
        public string? UserId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}