using Guru.DependencyInjection;
using Guru.EntityFramework.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace ConsoleApp.EntityFramework
{
    public class TestRunner
    {
        private readonly ICommandProvider _CommandProvider;

        public TestRunner()
        {
            _CommandProvider = ContainerEntry.Resolve<ICommandProvider>();
        }

        public void Run()
        {
            //var databaseProvider = ContainerEntry.Resolve<IDatabaseProvider>();
            //var db = databaseProvider.GetDatabase("sqlserver");

            try
            {
                var stopwatch = new Stopwatch();

                for (int i = 0; i < 100; i++)
                {
                    stopwatch.Restart();

                    var messages = GetMessages().GetAwaiter().GetResult();

                    stopwatch.Stop();
                    Console.WriteLine($"cost: {stopwatch.Elapsed.TotalMilliseconds}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        private async Task<List<Message>> GetMessages()
        {
            var command = _CommandProvider.GetCommand("messages");
            command.SetParameterValue("@Type", "Text");
            return await command.GetEntitiesAsync<Message>();
        }
    }
}