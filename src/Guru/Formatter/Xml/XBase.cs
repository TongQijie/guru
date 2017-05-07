using System.Text;

namespace Guru.Formatter.Xml
{
    public abstract class XBase
    {
        public byte[] Key { get; set; }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Key);
        }
    }
}