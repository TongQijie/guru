using Guru.ExtensionMethod;

namespace Guru.Formatter.Xml
{
    public static class XObjectExtensionMethod
    {
        public static XAttribute GetAttribute(this XObject xObject, string name)
        {
            if (xObject == null || !xObject.Elements.HasLength())
            {
                return null;
            }

            return xObject.Elements.FirstOrDefault(x => x is XAttribute && (x as XAttribute).KeyString == name) as XAttribute;
        }

        public static bool HasAttribute(this XObject xObject, string name)
        {
            if (xObject == null || !xObject.Elements.HasLength())
            {
                return false;
            }

            return xObject.Elements.Exists(x => x is XAttribute && (x as XAttribute).KeyString == name);
        }

        public static XObject GetObject(this XObject xObject, string name)
        {
            if (xObject == null || !xObject.Elements.HasLength())
            {
                return null;
            }

            return xObject.Elements.FirstOrDefault(x => x is XObject && (x as XObject).KeyString == name) as XObject;
        }

        public static bool HasObject(this XObject xObject, string name)
        {
            if (xObject == null || !xObject.Elements.HasLength())
            {
                return false;
            }

            return xObject.Elements.Exists(x => x is XObject && (x as XObject).KeyString == name);
        }

        public static XText GetText(this XObject xObject)
        {
            if (xObject == null || !xObject.Elements.HasLength())
            {
                return null;
            }

            return xObject.Elements.FirstOrDefault(x => x is XText) as XText;
        }

        public static bool HasText(this XObject xObject)
        {
            if (xObject == null || !xObject.Elements.HasLength())
            {
                return false;
            }

            return xObject.Elements.Exists(x => x is XText);
        }
    }
}
