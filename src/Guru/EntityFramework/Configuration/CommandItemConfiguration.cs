using System.Data;

using Guru.Formatter.Xml;

namespace Guru.EntityFramework.Configuration
{
    public class CommandItemConfiguration
    {
        [XmlProperty(Alias = "name", IsAttr = true)]
        public string Name { get; set; }

        [XmlProperty(Alias = "database", IsAttr = true)]
        public string Database { get; set; }

        [XmlProperty(Alias = "commandType", IsAttr = true)]
        public CommandType CommandType { get; set; }

        [XmlProperty(Alias = "commandText")]
        public string CommandText { get; set; }

        [XmlProperty(Alias = "params", ArrayElementName = "param")]
        public CommandParameterConfiguration[] Parameters { get; set; }
    }
}