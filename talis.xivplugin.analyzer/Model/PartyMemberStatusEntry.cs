// Talis.XIVPlugin.Analyzer
// PartyMemberStatusEntry.cs

using System;
using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class PartyMemberStatusEntry
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EncounterId { get; set; }

        [Indexed]
        public UInt32 TimeOffset { get; set; }

        [Indexed]
        public UInt32 PartyMemberId { get; set; }

        [Indexed]
        public UInt32 CasterId { get; set; }

        public double Duration { get; set; }

        public int StatusId { get; set; }

        [MaxLength(140)]
        public string StatusName { get; set; }

        [MaxLength(140)]
        public string TargetName { get; set; }

        [Indexed]
        public UInt32 TimeExpiredOffset { get; set; }
    }
}
