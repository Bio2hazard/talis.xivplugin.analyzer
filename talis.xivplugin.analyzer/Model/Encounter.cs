// Talis.XIVPlugin.Analyzer
// Encounter.cs

using System;
using SQLite;

namespace Talis.XIVPlugin.Analyzer.Model
{
    public class Encounter
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public UInt32 MapId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
