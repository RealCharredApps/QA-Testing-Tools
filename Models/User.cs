using System;

namespace QaMastery.Models
{
    public class User
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}