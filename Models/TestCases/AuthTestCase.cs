namespace QaMastery.Models.TestCases {
    public class AuthTestCase
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Description { get; set; } = "";
        
        public override string ToString() => Description;
    }
}