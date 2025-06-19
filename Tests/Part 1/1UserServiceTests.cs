using NUnit.Framework;
using NUnit.Framework.Internal;
using QaMastery.Models;
using QaMastery.Services.Core;
using QaMastery.Services.Security;
using QaMastery.Models.Results;
using QaMastery.Models.Enums;
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
    }
    //NEW FIXTURE 

    // CHALLENGE 2: Create a test that would fail if run after another test
    // Then fix it with proper setup/teardown
    [TestFixture]
    public class TestIsolationChallenge_FailUserTest
    {
        private UserService _userService = null!;
        private TestDatabase _testDatabase = null!;
        private List<string> _createdUserIds = null!;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            //need to get the services
            //need to get the database and expected array
            Console.WriteLine("Setting up db:");
            _testDatabase = new TestDatabase();
            _testDatabase.Connect();

            _userService = new UserService(_testDatabase);
            _createdUserIds = new List<string>();
        }

        [SetUp]
        public void TestSetup()
        {
            //now that fixture got the important stuff... 
            //need to start test with clean data
            _createdUserIds.Clear();
            Console.WriteLine("Starting: {TestContext.CurrentContext.Test.Name}");
        }

        [Test]
        [Category("DuplicateUser")]
        public void TestThatDependsOnCleanState()
        {
            // Write a test that assumes clean database state
            //need to create the expected array
            //need to pass to the test
            //need to create the initial user
            var testUser = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                Password = "SomePassword123!"
            };
            //need to test both were created // should fail
            // Create the user
            var result1 = _userService.CreateUser(testUser);

            // Verify it was created... 
            Assert.That(result1?.IsSuccess, Is.True, "User Created should succeed.");
            Assert.That(result1?.UserId, Is.Not.Null.And.Not.Empty, "Should Return Valid User ID");
            _createdUserIds?.Add(result1!.UserId!);

            //create dup
            var result2 = _userService.CreateUser(testUser);
            // Make it fail, then fix with proper setup
            //Assert.Fail("Implement this challenge");
            //need to show if the test failed or not // or as expected
            Console.WriteLine($"Second creation result: Success={result2?.IsSuccess}");
            Assert.That(result2?.IsSuccess, Is.False, "User dup should fail.");
            Console.WriteLine("✅ Duplicate detection working correctly!");

            // CRITICAL?? Track the cleanup
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
    }
}


