// Talis.XIVPlugin.Analyzer
// Analyzer.cs

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FFXIVAPP.Common.Core.Memory;
using NLog;
using SQLite;
using Talis.XIVPlugin.Analyzer.Helpers;
using Talis.XIVPlugin.Analyzer.Model;
using ChatLogEntry = Talis.XIVPlugin.Analyzer.Model.ChatLogEntry;

namespace Talis.XIVPlugin.Analyzer
{
    internal sealed class Analyzer : INotifyPropertyChanged
    {
        #region Logger

        private static Logger _logger;

        private static Logger Logger
        {
            get
            {
                if (FFXIVAPP.Common.Constants.EnableNLog)
                {
                    return _logger ?? (_logger = LogManager.GetCurrentClassLogger());
                }
                return null;
            }
        }

        #endregion

        #region Property Bindings

        private static Analyzer _instance;
        public readonly Stopwatch EncounterTimer = new Stopwatch();

        public bool PartyCaptured;
        public uint PlayerId;
        private Encounter _encounter;
        private SQLiteAsyncConnection _sqLiteConnection;

        public List<PartyMemberStatusEntry> partyMemberStatusEntries = new List<PartyMemberStatusEntry>();

        public static Analyzer Instance
        {
            get { return _instance ?? (_instance = new Analyzer()); }
        }

        public int EncounterId
        {
            get { return (_encounter != null) ? _encounter.Id : 0; }
        }

        #endregion

        #region Utility Functions

        public void Create()
        {
            _sqLiteConnection = new SQLiteAsyncConnection(Constants.BaseDirectory + "analyzer.s3db");

            _sqLiteConnection.CreateTableAsync<ChatLogEntry>()
                             .ContinueWith(results => LogHelper.Log(Logger, "ChatLogEntry Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<Encounter>()
                             .ContinueWith(results => LogHelper.Log(Logger, "Encounter Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<Events>()
                             .ContinueWith(results => LogHelper.Log(Logger, "Events Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<MonsterEntity>()
                             .ContinueWith(results => LogHelper.Log(Logger, "MonsterEntity Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<MonsterEntityUpdate>()
                             .ContinueWith(results => LogHelper.Log(Logger, "MonsterEntityUpdate Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<MonsterStatusEntry>()
                             .ContinueWith(results => LogHelper.Log(Logger, "MonsterStatusEntry Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<PartyMemberEntity>()
                             .ContinueWith(results => LogHelper.Log(Logger, "PartyMemberEntity Table Created", LogLevel.Trace));
            _sqLiteConnection.CreateTableAsync<PartyMemberStatusEntry>()
                             .ContinueWith(results => LogHelper.Log(Logger, "PartyMemberStatusEntry Table Created", LogLevel.Trace));
        }

        public void StartEncounter(uint mapId)
        {
            try
            {
                _encounter = new Encounter
                {
                    StartTime = DateTime.Now,
                    MapId = mapId
                };

                _sqLiteConnection.InsertAsync(_encounter)
                                 .ContinueWith(t => LogHelper.Log(Logger, "Encounter " + _encounter.Id + " started.", LogLevel.Trace));

                EncounterTimer.Start();
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        public void EndEncounter()
        {
            _encounter.EndTime = DateTime.Now;

            var tempEncounter = _encounter;

            _sqLiteConnection.UpdateAsync(_encounter)
                             .ContinueWith(t => LogHelper.Log(Logger, "Encounter " + tempEncounter.Id + " ended. Duration: " + (tempEncounter.EndTime - tempEncounter.StartTime).ToString(), LogLevel.Trace));

            _encounter = null;
            PartyCaptured = false;
            EncounterTimer.Reset();
        }

        #endregion

        public void NewPartyMemberStatusEntry(StatusEntry statusEntry, uint partyMemberId)
        {
            try
            {
                var partyMemberStatusEntry = new PartyMemberStatusEntry
                {
                    CasterId = statusEntry.CasterID,
                    Duration = statusEntry.Duration,
                    EncounterId = EncounterId,
                    PartyMemberId = partyMemberId,
                    StatusId = statusEntry.StatusID,
                    StatusName = statusEntry.StatusName,
                    TargetName = statusEntry.TargetName,
                    TimeOffset = Convert.ToUInt32(EncounterTimer.ElapsedMilliseconds)
                };

                _sqLiteConnection.InsertAsync(partyMemberStatusEntry)
                                 .ContinueWith(t => LogHelper.Log(Logger, "Status " + partyMemberStatusEntry.StatusName + " (" + partyMemberStatusEntry.StatusId + ") for party member " + partyMemberId + " captured for encounter " + EncounterId + ".", LogLevel.Trace));

                partyMemberStatusEntries.Add(partyMemberStatusEntry);
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        public void EndPartyMemberStatusEntry(PartyMemberStatusEntry partyMemberStatusEntry)
        {
            partyMemberStatusEntry.TimeExpiredOffset = Convert.ToUInt32(EncounterTimer.ElapsedMilliseconds);
            _sqLiteConnection.UpdateAsync(partyMemberStatusEntry)
                             .ContinueWith(t => LogHelper.Log(Logger, "Status " + partyMemberStatusEntry.StatusName + " (" + partyMemberStatusEntry.StatusId + ") expired on encounter " + EncounterId + ".", LogLevel.Trace));
            partyMemberStatusEntries.Remove(partyMemberStatusEntry);
        }

        public void CapturePartyMember(PartyEntity partyMember)
        {
            try
            {
                var partyMemberEntity = new PartyMemberEntity
                {
                    EncounterId = EncounterId,
                    Job = partyMember.Job.ToString(),
                    Name = partyMember.Name,
                    PlayerId = partyMember.ID,
                    TimeOffset = Convert.ToUInt32(EncounterTimer.ElapsedMilliseconds)
                };

                _sqLiteConnection.InsertAsync(partyMemberEntity)
                                 .ContinueWith(t => LogHelper.Log(Logger, "Party member " + partyMemberEntity.Name + "( " + partyMemberEntity.PlayerId + " ) captured for encounter " + EncounterId + ".", LogLevel.Trace));

                foreach (var statusEntry in partyMember.StatusEntries)
                {
                    NewPartyMemberStatusEntry(statusEntry, partyMemberEntity.PlayerId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
