using System;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;

namespace Watchman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("arguments is not valid. [source] [target]");
                return;
            }

            var context = ContainerManager.Default.Resolve<IContext>();
            context.Source = args[0].FullPath();
            context.Target = args[1].FullPath();

            var root = new WatchFolder("", null);

            while (Console.ReadKey().KeyChar != 'q')
            {
                ;
            }
        }
    }
}
