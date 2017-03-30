using System;

namespace GitDiff
{
    public interface IConfig
    {
        string GitPath { get; set; }

        string LocalGitDirectory { get; }

        string BranchName { get; }

        DateTime StartTime { get; }

        DateTime? EndTime { get; }

        string CompareTool { get; }

        string CompareToolParams { get; }
    }
}