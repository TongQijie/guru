using System;

using Guru.DependencyInjection.Attributes;

namespace GitDiff
{
    [StaticFile(typeof(IConfig), "./config.json")]
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