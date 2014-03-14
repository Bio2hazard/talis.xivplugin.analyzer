// Talis.XIVPlugin.Analyzer
// MonsterEntity.cs

using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class MonsterEntity
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
        public string ActionStatus { get; set; }

        public int CastingId { get; set; }

        public double CastingPercentage { get; set; }

        public double CastingProgress { get; set; }

        public double CastingTargetId { get; set; }

        public double CastingTime { get; set; }

        public double Distance { get; set; }

        public double Heading { get; set; }

        public int HPCurrent { get; set; }

        public int HPMax { get; set; }

        public bool IsCasting { get; set; }

        public bool IsClaimed { get; set; }

        public bool IsFate { get; set; }

        public bool IsValid { get; set; }

        [MaxLength(50)]
        public string Job { get; set; }

        public int Level { get; set; }

        public int ModelId { get; set; }

        [MaxLength(140)]
        public string Name { get; set; }

        public int NPCId1 { get; set; }

        public int NPCId2 { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public int TargetId { get; set; }

        [MaxLength(50)]
        public string TargetType { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }
    }
}
