using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Network;
using Guru.ExtensionMethod;
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
            using (var outputStream = new FileStream("./N_3CHNdliVs.jpg".FullPath(), FileMode.Create, FileAccess.Write))
            {
                using (var broker = new HttpBroker("https://images.unsplash.com/photo-1485809876425-9c96ea4132fe?ixlib=rb-0.3.5&q=80&fm=jpg&crop=entropy&cs=tinysrgb&s=98bb75f1c508abe5f8cc2bfff437aefa")
                .SetTimeout(TimeSpan.FromSeconds(120)))
                {
                    if (await broker.GetAsync() == 200)
                    {
                        var total = 0;
                    
                        await broker.GetBodyAsync(async (b, o, c) =>
                        {
                            await outputStream.WriteAsync(b, 0, c);

                            total += c;
                            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}{total}bytes");
                        }, 50 * 1024);
                    }
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