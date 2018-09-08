namespace Guru.AspNetCore.Implementation.Api.Metadata
{
    public class ApiMethodMetadata
    {
        public string MethodName { get; set; }

        public ApiParameterMetadata[] InputParameters { get; set; }

        public ApiParameterMetadata OutputParameter { get; set; }
    }
}