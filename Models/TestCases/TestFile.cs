using QaMastery.Models.TestCases;
using QaMastery.Models.Results;
using QaMastery.Services.Security;

namespace QaMastery.Models.TestCases
{
    public class TestFile
        {
            public string FileName { get; set; } = null!;
            public byte[] Content { get; set; } = Array.Empty<byte>();
            public string ContentType { get; set; } = null!;
            public bool ShouldBeBlocked { get; set; } = true;
            public string AttackType { get; set; } = null!;
        }
}