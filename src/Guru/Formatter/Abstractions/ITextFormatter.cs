using System.Text;

namespace Guru.Formatter.Abstractions
{
    public interface ITextFormatter : IFormatter
    {
        Encoding DefaultEncoding { get; set; }
    }
}