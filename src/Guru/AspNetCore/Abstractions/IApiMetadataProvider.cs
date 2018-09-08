namespace Guru.AspNetCore.Abstractions
{
    public interface IApiMetadataProvider
    {
        string GetListHtml();

        string GetMethodHtml(string serviceName, string methodName);
    }
}