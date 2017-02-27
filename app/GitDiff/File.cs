namespace GitDiff
{
    public class File
    {
        public int Index { get; set; }

        public Commit LatestCommit { get; set; }

        public Commit InitialCommit { get; set; }

        public Commit HistoryCommit { get; set; }

        public string LatestAction { get; set; }

        public string InitialAction { get; set; }

        public string Path { get; set; }

        public string Mark { get; set; }

        public string FileName
        {
            get
            {
                var index = Path.TrimEnd('/').LastIndexOf('/');
                if (index == -1)
                {
                    return Path;
                }
                else
                {
                    return Path.Substring(index + 1);
                }
            }
        }
    }
}