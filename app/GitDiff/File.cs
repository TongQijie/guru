namespace GitDiff
{
    public class File
    {
        public Commit LatestCommit { get; set; }

        public Commit InitialCommit { get; set; }

        public Commit HistoryCommit { get; set; }

        public string LatestAction { get; set; }

        public string InitialAction { get; set; }

        public string Path { get; set; }

        public string Mark { get; set; }
    }
}