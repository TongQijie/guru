using System.IO;
using System.Text;
using System.Diagnostics;

namespace GitDiff
{
    public class ProcessHelper
    {
        public ProcessHelper(string executable)
        {
            Executable = executable;
        }

        public ProcessHelper(string executable, string arguments) 
        {
            this.Executable = executable;
            this.Arguments = arguments;
        }

        public string Executable { get; private set; }

        public string Arguments { get; private set; }

        public ProcessHelper Add(string argument)
        {
            Arguments += " " + argument;
            return this;
        }

        public string ReadString(string workingDirectory = null)
        {
            var process = new Process();
            process.StartInfo.FileName = Executable;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.Start();

            using (var reader = process.StandardOutput)
            {
                return reader.ReadToEnd();
            }
        }

        public StreamReader ReadStream(string workingDirectory = null)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Executable;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();

            return process.StandardOutput;
        }

        public void Execute(string workingDirectory = null)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Executable;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.Start();
        }
    }
}