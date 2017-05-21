using System;
using System.Text;
using System.Collections.Generic;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.Formatter.Json;
using System.Collections;
using System.Xml.Serialization;
using Guru.Formatter.Xml;

namespace ConsoleApp.Formatter
{
    public class TestRunner
    {
        private readonly IJsonFormatter _JsonFormatter;

        private readonly IXmlFormatter _XmlFormatter;

        public TestRunner()
        {
            _JsonFormatter = ContainerManager.Default.Resolve<IJsonFormatter>();
            _XmlFormatter = ContainerManager.Default.Resolve<IXmlFormatter>();

            _JsonFormatter.DefaultEncoding = Encoding.UTF8;
            _JsonFormatter.OmitDefaultValue = false;
        }

        public async void Run()
        {
            //var o1 = new Item()
            //{
            //    StringValue = "hello, world.></<>!",
            //    IntegerValue = 100,
            //    DoubleValue = 100.111,
            //    DateValue = DateTime.Now,
            //    BooleanValue = true,
            //    SubItem = new SubItem()
            //    {
            //        StringValue = "SubItem",
            //        IntegerValue = null,
            //        DateValue = DateTime.Now,
            //        BooleanValue = true,
            //    },
            //    SubItems = new SubItem[]
            //    {
            //        new SubItem()
            //    {
            //        StringValue = "SubItem1",
            //        IntegerValue = 1,
            //        DateValue = DateTime.Now,
            //        BooleanValue = true,
            //    },
            //        new SubItem()
            //    {
            //        StringValue = "SubItem2",
            //        IntegerValue = 2,
            //        DateValue = DateTime.Now,
            //        BooleanValue = true,
            //    },
            //    }
            //};

            //var j1 = await _JsonFormatter.WriteStringAsync(o1, Encoding.UTF8);

            //var d1 = await _JsonFormatter.ReadObjectAsync<Item>(j1, Encoding.UTF8);

            //var items = new ItemCollection();
            //items.Add(o1);
            //items.Add(o1);

            //var xml = await _XmlFormatter.WriteStringAsync(items, Encoding.UTF8);

            //Console.WriteLine(xml);

            //await _XmlFormatter.ReadObjectAsync<ItemCollection>("<a d=\"d\\//><d\" e=\"ee\"><b f=\"ff\"><!--jj-->bb<i/></b><k><![CDATA[ll]]></k><c>cc</c><g/><h /></a>", Encoding.UTF8);

            var item = await _XmlFormatter.ReadObjectAsync<Item>("<Item d=\"123.1222\" b=\"true\"><s>abc</s><i>21</i><DateValue>2017-10-01</DateValue><sub><s>abdss</s></sub><subs><SubItem><s>ac</s></SubItem><SubItem><s>acfff</s></SubItem></subs></Item>", Encoding.UTF8);


            //Console.WriteLine(_XmlFormatter.WriteString(new Dictionary<string, string>()
            //{
            //    {"abc", "aaaaaa" },
            //    {"def", "aaaaa" }
            //}, Encoding.UTF8));


            // var subItem = new SubItem();
            // subItem.DateValue = DateTime.Now;
            // typeof(SubItem).GetProperty("DateValue").SetValue(subItem, d, null);
            // typeof(SubItem).GetProperty("BooleanValue").SetValue(subItem, true, null);



            //var dict1 = new Dictionary<string, string>()
            //{
            //    { "key1", "value1" },
            //    { "key2", "value2" },
            //};

            //var dict2 = new Dictionary<string, Item>()
            //{
            //    { "item1", item },
            //    { "item2", item },
            //};



            //// var items = new Item[] { item };

            //var items = new List<Item>() { item };

            //var json = await _JsonFormatter.WriteStringAsync(items, Encoding.UTF8);

            //var json1 = await _JsonFormatter.WriteStringAsync(dict1, Encoding.UTF8);
            //Console.WriteLine(json1);

            //var json2 = await _JsonFormatter.WriteStringAsync(dict2, Encoding.UTF8);
            //Console.WriteLine(json2);

            //var json3 = await _JsonFormatter.ReadObjectAsync<Dictionary<string, string>>(json1, Encoding.UTF8);

            //var json4 = await _JsonFormatter.ReadObjectAsync<Dictionary<string, Item>>(json2, Encoding.UTF8);
            //var i = await _JsonFormatter.ReadObjectAsync<Item>(json, Encoding.UTF8);
        }

        public class Item
        {
            [XmlProperty(Alias = "s")]
            public string StringValue { get; set; }

            [XmlProperty(Alias = "i")]
            public int IntegerValue { get; set; }

            [XmlProperty(Attribute = true, Alias = "d")]
            public double DoubleValue { get; set; }

            public DateTime DateValue { get; set; }

            [XmlProperty(Attribute = true, Alias = "b")]
            public bool BooleanValue { get; set; }

            [XmlProperty(Alias = "sub")]
            public SubItem SubItem { get; set; }

            [XmlProperty(Alias = "subs")]
            public SubItem[] SubItems { get; set; }

        }

        public class SubItem
        {
            [XmlProperty(Alias = "s")]
            public string StringValue { get; set; }

            public int? IntegerValue { get; set; }

            public DateTime? DateValue { get; set; }

            public bool? BooleanValue { get; set; }
        }

        [XmlRoot("items")]
        public class ItemCollection : List<Item>
        {
        }
    }
}