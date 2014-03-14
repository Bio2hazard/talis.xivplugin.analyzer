// Talis.XIVPlugin.Analyzer
// PartyMemberEntity.cs

using System;
using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class PartyMemberEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EncounterId { get; set; }

        [Indexed]
        public UInt32 TimeOffset { get; set; }

        [Indexed]
        public UInt32 PlayerId { get; set; }

        [MaxLength(50)]
        public string Job { get; set; }

        [MaxLength(140)]
        public string Name { get; set; }
    }
}
