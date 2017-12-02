using Guru.AspNetCore.Attributes;

namespace ClassLib
{
    [Api("test")]
    public class TestApi
    {
        [ApiMethod("hi")]
        public string Hi(string hi)
        {
            return hi;
        }
    }
}