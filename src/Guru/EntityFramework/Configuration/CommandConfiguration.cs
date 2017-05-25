using System.Collections.Generic;

using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Xml;

namespace Guru.EntityFramework.Configuration
{
    [StaticFile(typeof(ICommandConfiguration), "./Configuration/commands_*.xml", Format = "xml", MultiFiles = true)]
    [XmlClass(RootName = "commands", ArrayElementName = "command", ArrayElementType = typeof(CommandItemConfiguration))]
    public class CommandConfiguration : List<CommandItemConfiguration>, ICommandConfiguration
    {
        public CommandItemConfiguration[] Items { get { return this.ToArray(); } }
    }
}