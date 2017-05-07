using Guru.Formatter.Internal;
using System.Text;
using System.Threading.Tasks;

namespace Guru.Formatter.Xml
{
    public abstract class XBase
    {
        public byte[] Key { get; set; }

        internal abstract Task<bool> FillAsync(IReaderStream stream, byte[] terminators);

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Key);
        }
    }
}