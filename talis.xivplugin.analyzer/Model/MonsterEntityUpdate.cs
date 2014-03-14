// Talis.XIVPlugin.Analyzer
// MonsterEntityUpdate.cs

using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class MonsterEntityUpdate
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

        [MaxLength(50)]
        public string Field { get; set; }

        [MaxLength(140)]
        public string Value { get; set; }
    }
}
