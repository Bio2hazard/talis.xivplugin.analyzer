// Talis.XIVPlugin.Analyzer
// Events.cs

using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class Events
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EncounterId { get; set; }

        [Indexed]
        public int TimeOffset { get; set; }

        [Indexed]
        public int Code { get; set; }

        [Indexed]
        public int InvolvedEntityId { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }
    }
}
