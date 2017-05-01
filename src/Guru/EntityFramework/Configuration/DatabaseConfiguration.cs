using System.Xml.Serialization;

using Guru.DependencyInjection;

namespace Guru.EntityFramework.Configuration
{
    [FileDI(typeof(IDatabaseConfiguration), "./Configuration/databases.xml", Format = FileFormat.Xml)]
    [XmlRoot(ElementName = "databases")]
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        [XmlElement(ElementName = "database")]
        public DatabaseItemConfiguration[] Items { get; set; }
    }
}