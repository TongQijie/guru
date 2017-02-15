using System;
using System.IO;
using System.Linq;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace GitDiff
{
    [DI(typeof(IGit), Lifetime = Lifetime.Singleton)]
    public class Git : IGit
    {
        public string LocalDirectory { get; set; }

        public Change[] GetCommitDetail(string commitId)
        {
            var process = new ProcessHelper("git").Add("show")
                .Add("--pretty=\"\"").Add("--name-status")
                .Add(commitId);

            var changes = new Change[0];
            using (var reader = process.ReadStream(LocalDirectory))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.SplitByChar('\t');
                    if (fields.Length >= 2)
                    {
                        changes = changes.Append(new Change()
                        {
                            Action = fields[0],
                            Path = fields[1],
                        });
                    }
                }
            }

            return changes;
        }

        public Stream GetFileContent(string commitId, string path)
        {
            throw new NotImplementedException();
        }

        public Commit GetFileHistory(string path, Commit beforeCommit)
        {
            var process = new ProcessHelper("git").Add("log")
                .Add("--name-status")
                .Add("--before").Add($"\"{beforeCommit.Date.AddSeconds(-1)}\"")
                .Add("--").Add($"\"{path}\"");

            using (var reader = process.ReadStream(LocalDirectory))
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

        public Commit[] GetTotalCommits(DateTime startTime, DateTime endTime, string branchName)
        {
            var process = new ProcessHelper("git").Add("log")
                .Add("--after").Add($"\"{startTime.ToString("yyyy-MM-dd HH:mm:ss")}\"")
                .Add("--before").Add($"\"{endTime.ToString("yyyy-MM-dd HH:mm:ss")}\"")
                .Add("--pretty=format:\"%H|%cI|%an|%s\"")
                .Add(branchName);

            var commits = new Commit[0];
            using (var reader = process.ReadStream(LocalDirectory))
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