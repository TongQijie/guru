using System;
using System.Diagnostics;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using Guru.Util;

namespace ConsoleApp
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Executable : IConsoleExecutable
    {
        public int Run(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //new DependencyInjection.TestRunner().Run();

            //new Formatter.TestRunner().Run();

            //new Network.TestRunner().Run();

            // new Network.UnsplashCrawler().Run();

            // new EntityFramework.TestRunner().Run();

            // new Cache.TestRunner().Run();

            // new Mq.TestRunner().Run();

            //new Jobs.TestRunner().Run();

            //new Markdown.TestRunner().Run();

            stopwatch.Stop();
            //Console.WriteLine($"test done. cost: {stopwatch.Elapsed.TotalMilliseconds}ms.");

            return 0;
        }

        public static string PercentEncode(string value)
        {
            var stringBuilder = new StringBuilder();
            const string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        public static string SignString(string source, string accessSecret)
        {
            using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        public static string FormatIso8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
    }
}