using Guru.ExtensionMethod;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Guru.Utils
{
    public static class ProcessUtils
    {
        public static void Execute(string executable, string arguments, string workingDirectory)
        {
            if (!executable.HasValue())
            {
                throw new Exception("executable cannot be empty.");
            }

            var process = new Process();
            process.StartInfo.FileName = executable;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.Start();
        }

        public static StreamReader ExecuteToStream(string executable, string arguments, string workingDirectory)
        {
            if (!executable.HasValue())
            {
                throw new Exception("executable cannot be empty.");
            }

            var process = new Process();
            process.StartInfo.FileName = executable;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();

            return process.StandardOutput;
        }
    }
}