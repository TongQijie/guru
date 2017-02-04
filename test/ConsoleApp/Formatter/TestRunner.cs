using System.Text;

using Guru.Formatter.Abstractions;
using Guru.DependencyInjection;

namespace ConsoleApp.Formatter
{
    public class TestRunner
    {
        public async void Run()
        {
            var item = new Item()
            {
                StringValue = "hello, world.",
                IntegerValue = 100,
                DoubleValue = 100.111
            };

            var formatter = ContainerEntry.Resolve<IJsonFormatter>();

            var json = formatter.WriteString(item, Encoding.UTF8);

            var i = await formatter.ReadObjectAsync<Item>(json, Encoding.UTF8);
        }

        public class Item
        {
            public string StringValue { get; set; }

            public int IntegerValue { get; set; }

            public double DoubleValue { get; set; }
        }
    }
}
