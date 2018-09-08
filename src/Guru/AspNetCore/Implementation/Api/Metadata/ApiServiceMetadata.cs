namespace Guru.AspNetCore.Implementation.Api.Metadata
{
    public class ApiServiceMetadata
    {
        public string ServiceName { get; set; }

        public ApiMethodMetadata[] Methods { get; set; }
    }
}