namespace Guru.Formatter.Abstractions
{
    public interface IJsonLightningFormatter : ILightningFormatter
    {
        bool OmitDefaultValue { get; set; }
    }
}