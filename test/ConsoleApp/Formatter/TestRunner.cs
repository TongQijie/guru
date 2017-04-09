using System;
using System.Text;
using System.Collections.Generic;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

namespace ConsoleApp.Formatter
{
    public class TestRunner
    {
        private readonly IJsonFormatter _JsonFormatter;

        public TestRunner()
        {
            _JsonFormatter = ContainerEntry.Resolve<IJsonFormatter>();

            _JsonFormatter.DefaultEncoding = Encoding.UTF8;
            _JsonFormatter.OmitDefaultValue = false;
        }

        public async void Run()
        {
            //var d = new Nullable<DateTime>();
            // var d = DateTime.Now;

            // var subItem = new SubItem();
            // subItem.DateValue = DateTime.Now;
            // typeof(SubItem).GetProperty("DateValue").SetValue(subItem, d, null);
            // typeof(SubItem).GetProperty("BooleanValue").SetValue(subItem, true, null);

            var item = new Item()
            {
                StringValue = "hello, world.",
                IntegerValue = 100,
                DoubleValue = 100.111,
                DateValue = DateTime.Now,
                BooleanValue = true,
                SubItem = new SubItem()
                {
                    StringValue = "SubItem",
                    IntegerValue = null,
                    DateValue = DateTime.Now,
                    BooleanValue = true,
                },
            };

            // var items = new Item[] { item };

            var items = new List<Item>() { item };

            var json = await _JsonFormatter.WriteStringAsync(items, Encoding.UTF8);

            //var i = await _JsonFormatter.ReadObjectAsync<Item>(json, Encoding.UTF8);
        }

        public class Item
        {
            public string StringValue { get; set; }

            public int IntegerValue { get; set; }

            public double DoubleValue { get; set; }

            public DateTime DateValue { get; set; }

            public bool BooleanValue { get; set; }

            public SubItem SubItem { get; set; }
        }

        public class SubItem
        {
            public string StringValue { get; set; }

            public int? IntegerValue { get; set; }

            public DateTime? DateValue { get; set; }

            public bool? BooleanValue { get; set; }
        }
    }
}
