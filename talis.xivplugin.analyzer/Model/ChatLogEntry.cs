// Talis.XIVPlugin.Analyzer
// ChatLogEntry.cs

using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class ChatLogEntry
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EncounterId { get; set; }

        [Indexed]
        public int TimeOffset { get; set; }

        [Indexed]
        public int ChatCode { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }
    }
}
