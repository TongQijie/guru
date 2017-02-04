namespace Guru.Formatter.Abstractions
{
    public interface IXmlFormatter : IFormatter
    {
        bool OmitNamespaces { get; set; }
    }
}
