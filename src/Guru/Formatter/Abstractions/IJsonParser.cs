using System.IO;
using System.Threading.Tasks;

using Guru.Formatter.Json;

namespace Guru.Formatter.Abstractions
{
    public interface IJsonParser
    {
        JBase Parse(Stream stream);

        Task<JBase> ParseAsync(Stream stream);
    }
}
