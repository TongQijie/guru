using System;
using System.IO;

namespace GitDiff
{
    public interface IGit
    {
        string LocalDirectory { get; set; }

        Commit[] GetTotalCommits(DateTime startTime, DateTime endTime, string branchName);

        Change[] GetCommitDetail(string commitId);

        Commit GetFileHistory(string path, Commit beforeCommit);

        Stream GetFileContent(string commitId, string path);
    }
}