using System.IO;
using Guru.Hjson;

namespace Guru.Utils
{
    public static class HjsonUtils
    {
        public static string ToJson(Stream stream)
        {
            return HjsonValue.Load(stream, new HjsonOptions()).ToString();
        }
    }
}