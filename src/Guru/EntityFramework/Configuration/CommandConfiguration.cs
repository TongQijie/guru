using System.Xml.Serialization;
using System.Collections.Generic;

using Guru.DependencyInjection.Attributes;

namespace Guru.EntityFramework.Configuration
{
    [StaticFile(typeof(ICommandConfiguration), "./Configuration/commands_*.xml", Format = "xml", MultiFiles = true)]
    [XmlRoot(ElementName = "commands")]
    public class CommandConfiguration : List<CommandItemConfiguration>, ICommandConfiguration
    {
        public CommandItemConfiguration[] Items { get { return this.ToArray(); } }
    }
}