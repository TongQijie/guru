using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using ConsoleApp.Middleware;

using Guru.Network;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

namespace ConsoleApp.Network
{
    public class TestRunner
    {
        private readonly IHttpClientBroker _Broker;

        private IWebProxy GetProxy()
        {
            return new DefaultWebProxy("stfirewall", 8080, "jt69", "Newegg@12345", "buyabs.corp");
        }

        public TestRunner()
        {
            _Broker = ContainerEntry.Resolve<IHttpClientBroker>();
        }

        public void Run()
        {
            HttpBrokerTest().GetAwaiter().GetResult();
        }

        private async Task HttpBrokerTest()
        {
            var settings = new DefaultHttpClientSettings("default", null, GetProxy(), null);

            var request = _Broker.Get(settings);
            using (var response = await request.GetAsync("http://www.baidu.com", null))
            {
                if (response.StatusCode == 200)
                {
                    Console.WriteLine(await response.GetBodyAsync<string, ITextFormatter>());
                }
            }

            return;

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