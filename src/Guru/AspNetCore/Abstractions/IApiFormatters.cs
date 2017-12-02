using Guru.AspNetCore.Implementation.Api.Formatter;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiFormatters
    {
        AbstractApiFormatter Get(CallingContext context);

        AbstractApiFormatter Json { get; }

        AbstractApiFormatter Xml { get; }

        AbstractApiFormatter Text { get; }
    }
}