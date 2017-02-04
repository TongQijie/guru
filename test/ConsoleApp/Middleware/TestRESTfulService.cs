using System.Threading.Tasks;

using Guru.Middleware.RESTfulService;

namespace ConsoleApp.Middleware
{
    [Service("test")]
    public class TestRESTfulService
    {
        [Method(Name = "hi1", Response = ContentType.Text)]
        public string Hi()
        {
            return "hello, world!";
        }

        [Method(Name = "hi2", Response = ContentType.Text)]
        public string Hi(string welcome)
        {
            return welcome;
        }

        [Method(Name = "hi3", Request = ContentType.Json, Response = ContentType.Text)]
        public string Hi(Request request)
        {
            return request.Data;
        }

        [Method(Name = "hi4", Request = ContentType.Json, Response = ContentType.Text)]
        public string Hi(Request request, [Parameter(Alias = "word")] string welcome)
        {
            return $"{request?.Data},{welcome}";
        }

        [Method(Name = "hi5", Request = ContentType.Json, Response = ContentType.Json)]
        public Response Hi(Request request, [Parameter(Alias = "word")] string welcome, int number)
        {
            return new Response()
            {
                Result = $"{request?.Data},{welcome},{number}",
            };
        }

        [Method(Name = "hi6", Request = ContentType.Json, Response = ContentType.Json)]
        public async Task<Response> Hi(Request request, [Parameter(Alias = "word")] string welcome, int number, double price)
        {
            await Task.Delay(3000);

            return new Response()
            {
                Result = $"{request?.Data},{welcome},{number}",
            };
        }
    }

    public class Request
    {
        public string Data { get; set; }
    }

    public class Response
    {
        public string Result { get; set; }
    }
}