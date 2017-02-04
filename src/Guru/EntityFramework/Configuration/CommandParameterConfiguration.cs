using System.Data;
using System.Xml.Serialization;

namespace Guru.EntityFramework.Configuration
{
    public class CommandParameterConfiguration
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public DbType DbType { get; set; }

        [XmlAttribute(AttributeName = "direction")]
        public ParameterDirection Direction { get; set; }

        [XmlAttribute(AttributeName = "size")]
        public int Size { get; set; }
    }
}