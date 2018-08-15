using System;
using System.Collections.Generic;
using System.Text;

namespace Celebratus
{
    public class LexicalCategory
    {
        public Metadata Metadata { get; set; }
        public List<Result> Results { get; set; }
    }

    public class Metadata
    {
        public string Provider { get; set; }
    }

    public class Result
    {
        public string Id { get; set; }
        public string Language { get; set; }
        public List<LexicalEntry> LexicalEntries { get; set; }
        public string Type { get; set; }
        public string Word { get; set; }
    }

    public class LexicalEntry
    {
        public string Language { get; set; }
        public string LexicalCategory { get; set; }
        public string Text { get; set; }
    }
}
