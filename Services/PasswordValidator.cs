using System;

namespace QaMastery.Week1.Day1
{
    public class PasswordValidator
    {
        public ValidationResult ValidatePassword(string password)
        {
            // TODO: Implement password validation logic
            return new ValidationResult { IsValid = password.Length >= 8 };
        }
    }
}