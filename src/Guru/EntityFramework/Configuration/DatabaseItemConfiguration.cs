using System.Xml.Serialization;

namespace Guru.EntityFramework.Configuration
{
    public class DatabaseItemConfiguration
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "connectionString")]
        public string ConnectionString { get; set; }

        [XmlAttribute(AttributeName = "provider")]
        public string Provider { get; set; }
    }
}