using Guru.Formatter.Xml;

namespace Guru.EntityFramework.Configuration
{
    public class DatabaseItemConfiguration
    {
        [XmlProperty(Alias = "name", IsAttr = true)]
        public string Name { get; set; }

        [XmlProperty(Alias = "connectionString", IsAttr = true)]
        public string ConnectionString { get; set; }

        [XmlProperty(Alias = "provider", IsAttr = true)]
        public string Provider { get; set; }
    }
}