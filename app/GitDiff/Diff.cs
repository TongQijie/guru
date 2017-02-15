using System;
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

        public Diff(IGit git)
        {
            _Git = git;
        }

        public void Execute(Commit[] commits)
        {
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

            // print changes
            foreach (var file in files)
            {
                Console.WriteLine($"{file.InitialAction}\t{file.LatestAction}\t{file.Path}");
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
            // . -> A -> D: .

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
                    file.Mark = "D";
                    continue;
                }

                if ((file.InitialAction == "A" && file.LatestAction == "A") ||
                    (file.InitialAction == "A" && file.LatestAction == "M"))
                {
                    file.Mark = "A";
                    continue;
                }
            }

            // print results
            foreach (var file in files)
            {
                Console.WriteLine($"{file.Mark}\t{file.Path}");
            }
        }
    }
}