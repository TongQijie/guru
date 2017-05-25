using System.Data;

using Guru.Formatter.Xml;

namespace Guru.EntityFramework.Configuration
{
    public class CommandParameterConfiguration
    {
        [XmlProperty(Alias = "name", IsAttr = true)]
        public string Name { get; set; }

        [XmlProperty(Alias = "type", IsAttr = true)]
        public DbType DbType { get; set; }

        [XmlProperty(Alias = "direction", IsAttr = true)]
        public ParameterDirection Direction { get; set; }

        [XmlProperty(Alias = "size", IsAttr = true)]
        public int Size { get; set; }
    }
}