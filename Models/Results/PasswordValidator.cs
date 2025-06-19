using System;
using System.ComponentModel.DataAnnotations;

namespace QaMastery.Week1.Day1
{
    public class PasswordValidator
    {
        public ValidationResult ValidatePassword(string password)
        {
            // TODO: Implement password validation logic
            if (password != null && password.Length >= 8)
            {
                return ValidationResult.Success!;
            }
            else
            {
                return new ValidationResult("Password must be at least 8 characters long.");
            }
        }
    }
}