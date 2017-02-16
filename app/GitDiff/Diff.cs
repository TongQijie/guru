using System;
using System.IO;
using System.Linq;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace GitDiff
{
    [DI(typeof(IDiff), Lifetime = Lifetime.Singleton)]
    public class Diff : IDiff
    {
        private readonly IGit _Git;

        private readonly IFileManager _FileManager;

        public Diff(IGit git, IFileManager fileManager)
        {
            _Git = git;
            _FileManager = fileManager;
        }

        public void Execute()
        {
            var index = 1;
            var commits = _Git.GetTotalCommits().Each(x => x.Index = index++);

            Print(commits);

            int n;
            while ((n = ReadIgnore(commits.Length)) != 0)
            {
                switch (n)
                {
                    case -1:
                        {
                            Print(commits);
                            break;
                        }
                    case -2:
                        {
                            Console.WriteLine("your input is not valid.");
                            break;
                        }
                    default:
                        {
                            index = 1;
                            commits = commits.Remove(x => x.Index == n).Each(x => x.Index = index++);
                            Print(commits);
                            break;
                        }
                }
            }

            var files = new File[0];

            foreach (var commit in commits.OrderBy(x => x.Date))
            {
                var changes = _Git.GetCommitDetail(commit.Id);
                foreach (var change in changes)
                {
                    var exists = files.FirstOrDefault(x => x.Path.EqualsIgnoreCase(change.Path));

                    if (exists == null)
                    {
                        var file = new File()
                        {
                            InitialCommit = commit,
                            LatestCommit = commit,
                            InitialAction = change.Action,
                            LatestAction = change.Action,
                            Path = change.Path,
                        };
                        files = files.Append(file);
                    }
                    else
                    {
                        exists.LatestCommit = commit;
                        exists.LatestAction = change.Action;
                    }
                }
            }

            // A -> A -> A: N/A
            // A -> A -> M: N/A
            // A -> A -> D: N/A
            // A -> M -> A: Compare
            // A -> M -> M: Compare
            // A -> M -> D: Delete
            // A -> D -> A: Compare
            // A -> D -> M: Compare
            // A -> D -> D: Delete
            // M -> A -> A: N/A
            // M -> A -> M: N/A
            // M -> A -> D: N/A
            // M -> M -> A: Compare
            // M -> M -> M: Compare
            // M -> M -> D: Delete
            // M -> D -> A: Compare
            // M -> D -> M: Compare
            // M -> D -> D: Delete
            // . -> A -> A: Add
            // . -> A -> M: Add
            // . -> A -> D: Ignore

            foreach (var file in files)
            {
                if ((file.InitialAction == "M" && file.LatestAction == "A") ||
                    (file.InitialAction == "M" && file.LatestAction == "M") ||
                    (file.InitialAction == "D" && file.LatestAction == "A") ||
                    (file.InitialAction == "D" && file.LatestAction == "M"))
                {
                    var commit = _Git.GetFileHistory(file.Path, file.InitialCommit);
                    if (commit != null)
                    {
                        file.HistoryCommit = commit;
                    }

                    file.Mark = "M";
                    continue;
                }

                if ((file.InitialAction == "M" && file.LatestAction == "D") ||
                    (file.InitialAction == "D" && file.LatestAction == "D"))
                {
                    var commit = _Git.GetFileHistory(file.Path, file.InitialCommit);
                    if (commit != null)
                    {
                        file.HistoryCommit = commit;
                    }

                    file.Mark = "D";
                    continue;
                }

                if ((file.InitialAction == "A" && file.LatestAction == "A") ||
                    (file.InitialAction == "A" && file.LatestAction == "M"))
                {
                    file.Mark = "A";
                    continue;
                }

                if (file.InitialAction == "A" && file.LatestAction == "D")
                {
                    file.Mark = "I";
                    continue;
                }
            }

            index = 1;
            var compares = files.Where(x => x.Mark != "I").OrderBy(x => x.Mark).ToArray().Each(x => x.Index = index++);

            // print results
            Print(compares);

            // wait input from user
            while ((n = ReadCompare(compares.Length)) != 0)
            {
                switch (n)
                {
                    case -1:
                        {
                            Print(compares);
                            break;
                        }
                    case -2:
                        {
                            Console.WriteLine("your input is not valid.");
                            break;
                        }
                    default:
                        {
                            var compare = compares.FirstOrDefault(x => x.Index == n);
                            if (compare.Mark == "M" && compare.HistoryCommit != null)
                            {
                                Console.WriteLine($"compare {compare.HistoryCommit.Id} to {compare.LatestCommit.Id}");

                                if (!"./tempfiles".IsFolder())
                                {
                                    Directory.CreateDirectory("./tempfiles".FullPath());
                                }

                                var history = $"./tempfiles/{compare.HistoryCommit.Id}".FullPath();
                                using (var outputStream = new FileStream(history, FileMode.Create, FileAccess.Write))
                                {
                                    using (var writer = new StreamWriter(outputStream))
                                    {
                                        writer.Write(_Git.GetFileContent(compare.HistoryCommit.Id, compare.Path));
                                    }
                                }

                                var latest = $"./tempfiles/{compare.LatestCommit.Id}".FullPath();
                                using (var outputStream = new FileStream(latest, FileMode.Create, FileAccess.Write))
                                {
                                    using (var writer = new StreamWriter(outputStream))
                                    {
                                        writer.Write(_Git.GetFileContent(compare.LatestCommit.Id, compare.Path));
                                    }
                                }

                                var config = _FileManager.Single<IConfig>();

                                new ProcessHelper(config.CompareTool).Add(config.CompareToolParams ?? string.Empty).Add($"\"{history}\"").Add($"\"{latest}\"").Execute();
                            }
                            else if (compare.Mark == "A")
                            {
                                Console.WriteLine($"{compare.Path} Added.");

                                if (!"./tempfiles".IsFolder())
                                {
                                    Directory.CreateDirectory("./tempfiles".FullPath());
                                }

                                var history = "./tempfiles/empty".FullPath();
                                if (!history.IsFile())
                                {
                                    using (var outputStream = new FileStream(history, FileMode.Create, FileAccess.Write)) { }
                                }

                                var latest = $"./tempfiles/{compare.LatestCommit.Id}".FullPath();
                                using (var outputStream = new FileStream(latest, FileMode.Create, FileAccess.Write))
                                {
                                    using (var writer = new StreamWriter(outputStream))
                                    {
                                        writer.Write(_Git.GetFileContent(compare.LatestCommit.Id, compare.Path));
                                    }
                                }

                                var config = _FileManager.Single<IConfig>();

                                new ProcessHelper(config.CompareTool).Add(config.CompareToolParams ?? string.Empty).Add($"\"{history}\"").Add($"\"{latest}\"").Execute();
                            }
                            else if (compare.Mark == "D" && compare.HistoryCommit != null)
                            {
                                Console.WriteLine($"{compare.Path} Deleted.");

                                if (!"./tempfiles".IsFolder())
                                {
                                    Directory.CreateDirectory("./tempfiles".FullPath());
                                }
                                
                                var history = $"./tempfiles/{compare.HistoryCommit.Id}".FullPath();
                                using (var outputStream = new FileStream(history, FileMode.Create, FileAccess.Write))
                                {
                                    using (var writer = new StreamWriter(outputStream))
                                    {
                                        writer.Write(_Git.GetFileContent(compare.HistoryCommit.Id, compare.Path));
                                    }
                                }

                                var latest = "./tempfiles/empty".FullPath();
                                if (!latest.IsFile())
                                {
                                    using (var outputStream = new FileStream(latest, FileMode.Create, FileAccess.Write)) { }
                                }

                                var config = _FileManager.Single<IConfig>();

                                new ProcessHelper(config.CompareTool).Add(config.CompareToolParams ?? string.Empty).Add($"\"{history}\"").Add($"\"{latest}\"").Execute();
                            }

                            break;
                        }
                }
            }
        }

        private void Print(Commit[] commits)
        {
            foreach (var commit in commits)
            {
                Console.WriteLine($"{commit.Index}\t{commit.Author}\t{commit.Subject}");
            }
        }

        private void Print(File[] files)
        {
            Console.WriteLine("Index\tStatus\tPath");
            foreach (var file in files)
            {
                Console.Write($"{file.Index}\t");
                if (file.Mark == "A")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (file.Mark == "D")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.Write($"{file.Mark}\t");
                Console.ResetColor();
                Console.WriteLine($"{file.Path}");
            }
        }

        private int ReadIgnore(int max)
        {
            Console.Write("[L=list,C=continue] Ignore commit Index: ");
            var input = Console.ReadLine().Trim();
            if (input.EqualsIgnoreCase("c") || input.EqualsIgnoreCase("continue"))
            {
                return 0;
            }
            else if (input.EqualsIgnoreCase("l") || input.EqualsIgnoreCase("list"))
            {
                return -1;
            }
            else
            {
                int index;
                if (int.TryParse(input, out index) && index > 0 && index <= max)
                {
                    return index;
                }
                else
                {
                    return -2;
                }
            }
        }

        private int ReadCompare(int max)
        {
            Console.Write("[L=list,Q=quit] Compare file Index: ");
            var input = Console.ReadLine().Trim();
            if (input.EqualsIgnoreCase("q") || input.EqualsIgnoreCase("quit"))
            {
                return 0;
            }
            else if (input.EqualsIgnoreCase("l") || input.EqualsIgnoreCase("list"))
            {
                return -1;
            }
            else
            {
                int index;
                if (int.TryParse(input, out index) && index > 0 && index <= max)
                {
                    return index;
                }
                else
                {
                    return -2;
                }
            }
        }
    }
}