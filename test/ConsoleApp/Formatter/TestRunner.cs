using System.Collections.Generic;
using System.Text;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

namespace ConsoleApp.Formatter
{
    public class TestRunner
    {
        private readonly IJsonFormatter _JsonFormatter;

        private readonly IXmlFormatter _XmlFormatter;

        private readonly IJsonLightningFormatter _JsonLightningFormatter;

        public TestRunner()
        {
            _JsonFormatter = DependencyContainer.Resolve<IJsonFormatter>();
            _XmlFormatter = DependencyContainer.Resolve<IXmlFormatter>();
            _JsonLightningFormatter = DependencyContainer.Resolve<IJsonLightningFormatter>();
            _JsonLightningFormatter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            _JsonLightningFormatter.OmitDefaultValue = true;

            _JsonFormatter.DefaultEncoding = Encoding.UTF8;
            _JsonFormatter.OmitDefaultValue = false;
        }

        public void Run()
        {
            var json = "{\"Body\":[[\"abc\",\"def\"],[\"ghi\",\"\\u003cem class=\\u0027datanull\\u0027\\u003eNULL\\u003c/em\\u003e\"]]}";
            var testA = _JsonLightningFormatter.ReadObject<TestA>(json);

            var json1 = _JsonLightningFormatter.WriteObject(testA);
        }

        public class TestA
        {
            public string[][] Body { get; set; }
        }
    }
}