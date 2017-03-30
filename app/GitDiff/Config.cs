using System;

using Guru.DependencyInjection;

namespace GitDiff
{
    [FileDI(typeof(IConfig), "./config.json", Format = FileFormat.Json)]
    public class Config : IConfig
    {
        public string GitPath { get; set; }

        public string LocalGitDirectory { get; set; }

        public string BranchName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string CompareTool { get; set; }

        public string CompareToolParams { get; set; }
    }
}