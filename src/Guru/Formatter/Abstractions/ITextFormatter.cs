using System.Text;

namespace Guru.Formatter.Abstractions
{
    public interface ITextFormatter : IFormatter
    {
        Encoding TextEncoding { get; set; }
    }
}