using System.Xml.Serialization;

using Guru.DependencyInjection;

namespace Guru.EntityFramework.Configuration
{
    [FileDI(typeof(ICommandConfiguration), "./Configuration/commands_*.xml", Format = FileFormat.Xml, Multiply = true)]
    [XmlRoot(ElementName = "commands")]
    public class CommandConfiguration : ICommandConfiguration
    {
        [XmlElement(ElementName = "command")]
        public CommandItemConfiguration[] Items { get; set; }
    }
}