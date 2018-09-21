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

        public static byte[] Write(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            using (var outputStream = new MemoryStream())
            {
                var fiWriter = XmlWriter.Create(new FIWriter(outputStream));
                doc.WriteTo(fiWriter);
                fiWriter.Close();

                return outputStream.ToArray();
            }
        }
    }
}