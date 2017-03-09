using System;
using StackExchange.Redis;

namespace ConsoleApp.Cache
{
    public class TestRunner
    {
        public void Run()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect("192.168.0.102:6379,password=123456");
                var db = redis.GetDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}