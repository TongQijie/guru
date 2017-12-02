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
            _JsonFormatter = ContainerManager.Default.Resolve<IJsonFormatter>();
            _XmlFormatter = ContainerManager.Default.Resolve<IXmlFormatter>();
            _JsonLightningFormatter = ContainerManager.Default.Resolve<IJsonLightningFormatter>();
            _JsonLightningFormatter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            _JsonLightningFormatter.OmitDefaultValue = true;

            _JsonFormatter.DefaultEncoding = Encoding.UTF8;
            _JsonFormatter.OmitDefaultValue = false;
        }

        public void Run()
        {
            
        }
    }
}