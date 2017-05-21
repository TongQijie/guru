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

        public XmlClass(Type classType, string arrayElementName, Type arrayElementType) : this(classType)
        {
            ArrayElementName = arrayElementName;
            ArrayElementType = arrayElementType;
        }

        public string Key
        {
            get
            {
                if (XmlType == XType.Array)
                {
                    return ArrayElementName.HasValue() ? ArrayElementName : ArrayElementType.Name;
                }

                return string.Empty;
            }
        }

        public Type ClassType { get; private set; }

        public XType XmlType { get; private set; }

        public string ArrayElementName { get; private set; }

        public Type ArrayElementType { get; private set; }
    }
}
