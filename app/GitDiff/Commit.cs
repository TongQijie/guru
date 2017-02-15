using System;

namespace GitDiff
{
    public class Commit
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        public string Subject { get; set; }
    }
}