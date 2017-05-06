using System.Xml.Serialization;

using Guru.DependencyInjection.Attributes;

namespace Guru.EntityFramework.Configuration
{
    [StaticFile(typeof(IDatabaseConfiguration), "./Configuration/databases.xml", Format = "xml")]
    [XmlRoot(ElementName = "databases")]
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        [XmlElement(ElementName = "database")]
        public DatabaseItemConfiguration[] Items { get; set; }
    }
}