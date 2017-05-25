using Guru.Formatter.Xml;
using Guru.DependencyInjection.Attributes;

namespace Guru.EntityFramework.Configuration
{
    [StaticFile(typeof(IDatabaseConfiguration), "./Configuration/databases.xml", Format = "xml")]
    [XmlClass(RootName = "databases")]
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        [XmlProperty(IsArrayElement = true, ArrayElementName = "database")]
        public DatabaseItemConfiguration[] Items { get; set; }
    }
}