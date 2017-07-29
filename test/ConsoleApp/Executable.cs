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
        public void Run(string[] args)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "Format", "JSON" },
                { "Version", "2015-01-09" },
                { "Timestamp", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
                { "SignatureMethod", "HMAC-SHA1" },
                { "SignatureVersion", "1.0" },
                { "SignatureNonce", Guid.NewGuid().ToString() },
                { "AccessKeyId", "LTAIfovT8kFSRmVk" },
                { "RegionId", "cn-hangzhou" },
            };

            //parameters.Add("Action", "AddDomainRecord");
            //parameters.Add("DomainName", "hephap.com");
            //parameters.Add("RR", "test");
            //parameters.Add("Type", "A");
            //parameters.Add("Value", "101.86.236.162");

            //parameters.Add("Action", "DeleteDomainRecord");
            //parameters.Add("RecordId", "3481792745052160");

            //parameters.Add("Action", "DescribeDomainRecords");
            //parameters.Add("DomainName", "hephap.com");

            //parameters.Add("Action", "UpdateDomainRecord");
            //parameters.Add("RecordId", "3481327461193728");
            //parameters.Add("RR", "blog");
            //parameters.Add("Type", "A");
            //parameters.Add("Value", "101.86.236.100");

            var sortedParameters = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);

            var queries = string.Join("&", sortedParameters.Select(x => string.Format("{0}={1}", PercentEncode(x.Key), PercentEncode(x.Value))));

            var stringToSign = "GET&" + PercentEncode("/") + "&" + PercentEncode(queries);

            var signature = SignString(stringToSign, "VJ1EsKwugLsuWPWSrPNnajEYy9ls07&");

            parameters.Add("Signature", signature);

            var url = "http://alidns.aliyuncs.com/?" + string.Join("&", parameters.Select(x => string.Format("{0}={1}", WebUtil.UrlEncode(x.Key), WebUtil.UrlEncode(x.Value))));

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