using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable.Abstractions;
using Guru.ExtensionMethod;
using System;

namespace Guru.Executable.Implementation
{
    [Injectable(typeof(ICommandLineArgsParser), Lifetime.Singleton)]
    public class DefaultCommandLineArgsParser : ICommandLineArgsParser
    {
        public CommandLineArgs Parse(string[] args)
        {
            var commandLineArgs = new CommandLineArgs(args);

            if (!args.HasLength())
            {
                return commandLineArgs;
            }

            string name = null;
            foreach (var arg in args)
            {
                if (name == null)
                {
                    if (arg.StartsWith("-") && arg.Length > 1)
                    {
                        name = arg.Substring(1);
                    }
                    else if (arg.StartsWith("--") && arg.Length > 2)
                    {
                        name = arg.Substring(2);
                    }
                    else
                    {
                        return commandLineArgs;
                    }
                }
                else
                {
                    commandLineArgs.Add(name, arg);
                    name = null;
                }
            }

            return commandLineArgs;
        }
    }
}