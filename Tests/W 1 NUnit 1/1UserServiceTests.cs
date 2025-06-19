using NUnit.Framework; 
using System;
using System.Collections.Generic;

namespace QaMastery.Week1.Day1
{
    [TestFixture]
    [Category("Practice: UserServiceTest")]
    public class PracticeUserTests
    {
        // init vars - like c# of course 
        private UserService _userService = null!;
        private TestDatabase _testDatabase = null!;
        private List<string> _createdUserIds = null!;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // init resources once for all tests
            Console.WriteLine("Setting up db connection");
            _testDatabase = new TestDatabase();
            _testDatabase.Connect();

            // Initialize user service and created user ids list
            _userService = new UserService(_testDatabase);
            _createdUserIds = new List<string>();
        }
        [SetUp]
        public void TestSetup()
        {
            _createdUserIds.Clear();
            Console.WriteLine("Starting: {TestContext.CurrentContext.Test.Name}");
        }

        // CHALLENGE 1: Build a test with proper cleanup
        // Create a test that:
        [Test]
        [Category("CreateUser")]
        public void CreateUser_ValidData_ShouldSucceedAndCleanup()
        {
            // We Need to make: array with this data: 
            // username
            // email
            // password
            // CreateUser... 
            // ShowValidation... 
            var testUser = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                Password = "SecurePassword123!"
            };

            // Create the user
            var result = _userService.CreateUser(testUser);

            // Verify it was created... 
            Assert.That(result?.IsSuccess, Is.True, "User Created should succeed.");
            Assert.That(result?.UserId, Is.Not.Null.And.Not.Empty, "Should Return Valid User ID");

            // CRITICAL?? Track the cleanup
 
                _createdUserIds?.Add(result!.UserId!);

            // VERIFY: User actually exists in the program
            if (result != null && !string.IsNullOrEmpty(result.UserId))
            {
                var retrievedUser = _userService!.GetUser(result.UserId);
                Assert.That(retrievedUser?.Email, Is.EqualTo(testUser.Email), "Retrieved should match");
                Console.WriteLine($"Successfully Created User: {result.UserId}");
            }
            else
            {
                Assert.Fail("User creation failed or UserId is null/empty.");
            }
        }

        [TearDown]
        public void TestCleanup()
        {
            // cleaup user data created during this test...
            Console.WriteLine("Cleaning up test data");
            foreach (var userId in _createdUserIds!)
            {
                _userService?.DeleteUser(userId);
            }
            Console.WriteLine($"Cleanedup {TestContext.CurrentContext.Test.Name} data");
        }

        [OneTimeTearDown]
        public void FixtureCleanup()
        {
            // close resources from above
            Console.WriteLine("Closing connections...");
            _testDatabase?.Disconnect();
        }

        // CHALLENGE 2: Create a test that would fail if run after another test
        // Then fix it with proper setup/teardown

        [Test]
        public void TestThatDependsOnCleanState()
        {
            // TODO: Write a test that assumes clean database state
            // Make it fail, then fix with proper setup
            Assert.Fail("Implement this challenge");
        }

    }
}