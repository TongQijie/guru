using Guru.FastInfoset;
using System.IO;
using System.Xml;

namespace Guru.Utils
{
    public static class FiUtils
    {
        public static string Read(byte[] data)
        {
            var doc = new XmlDocument();
            using (var inputStream = new MemoryStream(data))
            {
                XmlReader fiReader = XmlReader.Create(new FIReader(inputStream), null);
                doc.Load(fiReader);
                fiReader.Close();
            }
            return doc.OuterXml;
        }
    }
}
