// Talis.XIVPlugin.Analyzer
// MonsterStatusEntry.cs

using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class MonsterStatusEntry
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EncounterId { get; set; }

        [Indexed]
        public int TimeOffset { get; set; }

        [Indexed]
        public int MobId { get; set; }

        [Indexed]
        public int CasterId { get; set; }

        public double Duration { get; set; }

        public int StatusId { get; set; }

        [MaxLength(140)]
        public string TargetName { get; set; }

        [Indexed]
        public int TimeExpiredOffset { get; set; }
    }
}
