using Guru.ExtensionMethod;
using System;

namespace Guru.Formatter.Xml
{
    internal class XmlClass
    {
        private XmlClass(Type classType)
        {
            ClassType = classType;
            XmlType = XmlUtility.GetXmlType(ClassType);
        }

        public XmlClass(Type classType, string arrayElementName, Type arrayElementType, string rootName) : this(classType)
        {
            ArrayElementName = arrayElementName;
            ArrayElementType = arrayElementType;
            RootName = rootName;
        }

        public XmlClass(Type classType, string rootName)
        {
            RootName = rootName;
        }

        public string Key
        {
            get
            {
                if (XmlType == XType.Array)
                {
                    return ArrayElementName.HasValue() ? ArrayElementName : ArrayElementType.Name;
                }
                else
                {
                    return RootName.HasValue() ? RootName : ClassType.Name;
                }
            }
        }

        public Type ClassType { get; private set; }

        public XType XmlType { get; private set; }

        public string ArrayElementName { get; private set; }

        public Type ArrayElementType { get; private set; }

        public string RootName { get; set; }
    }
}
