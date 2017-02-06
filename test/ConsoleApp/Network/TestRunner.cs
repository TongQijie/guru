using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Network;
using Guru.Formatter.Abstractions;

using ConsoleApp.Middleware;

namespace ConsoleApp.Network
{
    public class TestRunner
    {
        public void Run()
        {
            HttpBrokerTest().GetAwaiter().GetResult();
        }

        private async Task HttpBrokerTest()
        {
            var host = "http://localhost:5000";

            using (var broker = new HttpBroker($"{host}/test/hi1"))
            {
                if (await broker.GetAsync() == 200)
                {
                    var text = await broker.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var broker = new HttpBroker($"{host}/test/hi2", new Dictionary<string, string>()
            {
                { "welcome", "abc" }
            }))
            {
                if (await broker.GetAsync() == 200)
                {
                    var text = await broker.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var broker = new HttpBroker($"{host}/test/hi3"))
            {
                if (await broker.PostAsync<IJsonFormatter>(new Request() { Data = "hello, world!" }) == 200)
                {
                    var text = await broker.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var broker = new HttpBroker($"{host}/test/hi4", new Dictionary<string, string>()
            {
                { "word", "abc" },
                { "welcome", "def" }
            }))
            {
                if (await broker.PostAsync<IJsonFormatter>(new Request() { Data = "hello, world!" }) == 200)
                {
                    var text = await broker.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var broker = new HttpBroker($"{host}/test/hi5", new Dictionary<string, string>()
            {
                { "word", "abc" },
                { "welcome", "def" },
                { "number", "123" }
            }))
            {
                if (await broker.PostAsync<IJsonFormatter>(new Request() { Data = "hello, world!" }) == 200)
                {
                    var response = await broker.GetBodyAsync<Response, IJsonFormatter>();

                    Console.WriteLine(response.Result);
                }
            }

            using (var broker = new HttpBroker($"{host}/test/hi6", new Dictionary<string, string>()
            {
                { "word", "abc" },
                { "welcome", "def" },
                { "number", "123" },
                { "price", "12.3" }
            }))
            {
                if (await broker.PostAsync<IJsonFormatter>(new Request() { Data = "hello, world!" }) == 200)
                {
                    var response = await broker.GetBodyAsync<Response, IJsonFormatter>();

                    Console.WriteLine(response.Result);
                }
            }


        }
    }
}