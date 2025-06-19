using System;

namespace QaMastery.Services.Core
{
    public class TestDatabase
    {
        public void Connect()
        {
            Console.WriteLine("Connected to test database");
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnected from test database");
        }
    }
}