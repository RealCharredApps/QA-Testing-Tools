using System;

namespace QaMastery.Models.Results{
    public class CreateUserResult
    {
        public bool IsSuccess { get; set; }
        public string? UserId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}