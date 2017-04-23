using Guru.Markdown;

namespace ConsoleApp.Markdown
{
    public class TestRunner
    {
        public void Run()
        {
            var markdown = "# 1234";
            var html = new MarkdownUtils().Transform(markdown);
        }
    }
}
