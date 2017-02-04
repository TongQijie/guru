using System;

namespace ConsoleApp
{
    public static class Assert
    {
        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                Console.WriteLine("failed.");
            }
        }
    }
};