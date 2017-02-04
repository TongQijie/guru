using System.Data;
using System.Xml.Serialization;

namespace Guru.EntityFramework.Configuration
{
    public class CommandItemConfiguration
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "database")]
        public string Database { get; set; }

        [XmlAttribute(AttributeName = "commandType")]
        public CommandType CommandType { get; set; }

        [XmlElement(ElementName = "commandText")]
        public string CommandText { get; set; }

        [XmlArray(ElementName = "params")]
        [XmlArrayItem(ElementName = "param")]
        public CommandParameterConfiguration[] Parameters { get; set; }
    }
}