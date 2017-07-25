namespace Guru.AspNetCore.Configuration
{
    public class ApiConfiguration
    {
        public ApiConfiguration()
        {
            Prefix = "api";
        }

        public string Prefix { get; set; }
    }
}