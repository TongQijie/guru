using System.Threading.Tasks;

using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    public abstract class JBase
    {
        public byte[] Key { get; set; }

        /// <summary>
        /// fill object content, including internal elements or value
        /// </summary>
        /// <param name="stream">object data stream</param>
        /// <param name="seperators">bytes that indicate external object can continue to fill next object.</param>
        /// <param name="terminators">bytes that indicate current filling object is the last object in external object.</param>
        /// <returns>if true, indicate current filling object is the last object in external object, else false</returns>
        internal abstract bool Fill(IReaderStream stream, byte[] seperators, byte[] terminators);

        internal abstract Task<bool> FillAsync(IReaderStream stream, byte[] seperators, byte[] terminators);
    }
}