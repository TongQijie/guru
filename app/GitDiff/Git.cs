using System;
using System.Linq;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace GitDiff
{
    [DI(typeof(IGit), Lifetime = Lifetime.Singleton)]
    public class Git : IGit
    {
        private readonly IFileManager _FileManager;

        public Git(IFileManager fileManager)
        {
            _FileManager = fileManager;
        }

        public Change[] GetCommitDetail(string commitId)
        {
            var process = new ProcessHelper("git").Add("show")
                .Add("--pretty=\"\"").Add("--name-status")
                .Add(commitId);

            var changes = new Change[0];
            using (var reader = process.ReadStream(_FileManager.Single<IConfig>().LocalGitDirectory))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.SplitByChar('\t');
                    if (fields.Length >= 2)
                    {
                        if (fields[0].StartsWith("R", StringComparison.OrdinalIgnoreCase))
                        {
                            changes = changes.Append(new Change()
                            {
                                Action = "D",
                                Path = fields[1],
                            }).Append(new Change()
                            {
                                Action = "A",
                                Path = fields[2],
                            });
                        }
                        else if(fields[0] == "MM")
                        {
                            changes = changes.Append(new Change()
                            {
                                Action = "M",
                                Path = fields[1],
                            });
                        }
                        else
                        {
                            changes = changes.Append(new Change()
                            {
                                Action = fields[0],
                                Path = fields[1],
                            });
                        }
                    }
                }
            }

            return changes;
        }

        public string GetFileContent(string commitId, string path)
        {
            return new ProcessHelper("git").Add("show").Add($"{commitId}:\"{path}\"")
                .ReadString(_FileManager.Single<IConfig>().LocalGitDirectory);
        }

        public Commit GetFileHistory(string path, Commit beforeCommit)
        {
            var process = new ProcessHelper("git").Add("log")
                .Add("--name-status")
                .Add("--before").Add($"\"{beforeCommit.Date.AddSeconds(-1)}\"")
                .Add("--").Add($"\"{path}\"");

            using (var reader = process.ReadStream(_FileManager.Single<IConfig>().LocalGitDirectory))
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    return null;
                }

                var commit = new Commit();
                if (line.StartsWith("commit", StringComparison.OrdinalIgnoreCase))
                {
                    commit.Id = line.Substring(line.IndexOf(' ')).Trim();
                }

                line = reader.ReadLine();
                if (line.StartsWith("Author", StringComparison.OrdinalIgnoreCase))
                {
                    commit.Author = line.Substring(line.IndexOf(' ')).Trim();
                }

                line = reader.ReadLine();
                if (line.StartsWith("Date", StringComparison.OrdinalIgnoreCase))
                {
                    //commit.Date = DateTime.Parse(line.Substring(line.IndexOf(' ')).Trim());
                }

                reader.ReadLine();
                commit.Subject = reader.ReadLine();

                return commit;
            }
        }

        public Commit[] GetTotalCommits()
        {
            var config = _FileManager.Single<IConfig>();

            var process = new ProcessHelper("git").Add("log")
                .Add("--after").Add($"\"{config.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}\"")
                .Add("--before").Add($"\"{(config.EndTime ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")}\"")
                .Add("--pretty=format:\"%H|%cI|%an|%s\"")
                .Add(config.BranchName);

            var commits = new Commit[0];
            using (var reader = process.ReadStream(_FileManager.Single<IConfig>().LocalGitDirectory))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.SplitByChar('|');
                    if (fields.Length >= 4)
                    {
                        commits = commits.Append(new Commit()
                        {
                            Id = fields[0],
                            Date = DateTime.Parse(fields[1]),
                            Author = fields[2],
                            Subject = string.Join("|", fields.Skip(3)),
                        });
                    }
                }
            }

            return commits;
        }
    }
}