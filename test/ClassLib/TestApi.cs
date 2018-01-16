using Guru.AspNetCore.Attributes;

namespace ClassLib
{
    [Api("testApi")]
    public class TestApi
    {
        [ApiMethod("sayHi")]
        public string SayHi(string word)
        {
            return word;
        }

        [ApiMethod("toDo")]
        public void ToDo(string something)
        {

        }
    }
}