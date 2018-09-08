using Guru.AspNetCore.Implementation.Api.Formatter;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiFormatterProvider
    {
        AbstractApiFormatter Get(CallingContext context);

        AbstractApiFormatter Json { get; }

        AbstractApiFormatter Xml { get; }

        AbstractApiFormatter Text { get; }
    }
}