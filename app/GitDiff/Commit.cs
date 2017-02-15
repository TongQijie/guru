using System;

namespace GitDiff
{
    public class Commit
    {
        public int Index { get; set; }

        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        public string Subject { get; set; }
    }
}