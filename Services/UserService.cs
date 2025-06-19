using System;
using System.Collections.Generic;

namespace QaMastery.Week1.Day1
{
    public class UserService
    {
        private TestDatabase _database;
        private Dictionary<string, User> _userStorage = new(); // Add this storage

        public UserService(TestDatabase database)
        {
            _database = database;
        }

        public CreateUserResult CreateUser(User user)
        {
            Console.WriteLine($"Creating user: {user.Email}");

            // Check for duplicate emails in our storage
            foreach (var existingUser in _userStorage.Values)
            {
                if (existingUser.Email == user.Email)
                {
                    return new CreateUserResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "User with this email already exists"
                    };
                }
            }

            // Create and store the user
            var userId = Guid.NewGuid().ToString();
            _userStorage[userId] = new User
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password
            };

            Console.WriteLine($"✅ User created with ID: {userId}");

            return new CreateUserResult
            {
                IsSuccess = true,
                UserId = userId
            };
        }

        public User GetUser(string userId)
        {
            Console.WriteLine($"Retrieving user: {userId}");

            // Actually look up the user we stored
            if (_userStorage.TryGetValue(userId, out var user))
            {
                Console.WriteLine($"✅ Found user: {user.Email}");
                return user;
            }

            Console.WriteLine($"❌ User not found with ID: {userId}");
            // Return empty user if not found (this will make tests fail appropriately)
            return new User
            {
                Email = string.Empty,
                Name = string.Empty,
                Password = string.Empty
            };
        }

        public bool DeleteUser(string userId)
        {
            Console.WriteLine($"🗑️ Deleting user: {userId}");
            return _userStorage.Remove(userId);
        }
    }
}