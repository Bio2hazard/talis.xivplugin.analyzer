// Talis.XIVPlugin.Analyzer
// EventSubscriber.cs

using System;
using System.Linq;
using FFXIVAPP.IPluginInterface.Events;
using NLog;
using Talis.XIVPlugin.Analyzer.Helpers;
using Talis.XIVPlugin.Analyzer.Utilities;

namespace Talis.XIVPlugin.Analyzer
{
    public static class EventSubscriber
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

        public static void Subscribe()
        {
            Plugin.PHost.NewConstantsEntity += OnNewConstantsEntity;
            Plugin.PHost.NewChatLogEntry += OnNewChatLogEntry;
            Plugin.PHost.NewMonsterEntries += OnNewMonsterEntries;
            Plugin.PHost.NewNPCEntries += OnNewNPCEntries;
            Plugin.PHost.NewPCEntries += OnNewPCEntries;
            Plugin.PHost.NewPlayerEntity += OnNewPlayerEntity;
            Plugin.PHost.NewTargetEntity += OnNewTargetEntity;
            Plugin.PHost.NewParseEntity += OnNewParseEntity;
            Plugin.PHost.NewPartyEntries += OnNewPartyEntries;
        }

        public static void UnSubscribe()
        {
            Plugin.PHost.NewConstantsEntity -= OnNewConstantsEntity;
            Plugin.PHost.NewChatLogEntry -= OnNewChatLogEntry;
            Plugin.PHost.NewMonsterEntries -= OnNewMonsterEntries;
            Plugin.PHost.NewNPCEntries -= OnNewNPCEntries;
            Plugin.PHost.NewPCEntries -= OnNewPCEntries;
            Plugin.PHost.NewPlayerEntity -= OnNewPlayerEntity;
            Plugin.PHost.NewTargetEntity -= OnNewTargetEntity;
            Plugin.PHost.NewParseEntity -= OnNewParseEntity;
            Plugin.PHost.NewPartyEntries -= OnNewPartyEntries;
        }

        #region Subscriptions

        private static void OnNewConstantsEntity(object sender, ConstantsEntityEvent constantsEntityEvent)
        {
            // delegate event from constants, not required to subsribe, but recommended as it gives you app settings
            if (sender == null)
            {
                return;
            }
            var constantsEntity = constantsEntityEvent.ConstantsEntity;
            Constants.AutoTranslate = constantsEntity.AutoTranslate;
            Constants.ChatCodes = constantsEntity.ChatCodes;
            Constants.Colors = constantsEntity.Colors;
            Constants.CultureInfo = constantsEntity.CultureInfo;
            Constants.CharacterName = constantsEntity.CharacterName;
            Constants.ServerName = constantsEntity.ServerName;
            Constants.GameLanguage = constantsEntity.GameLanguage;
            Constants.EnableHelpLabels = constantsEntity.EnableHelpLabels;
            PluginViewModel.Instance.EnableHelpLabels = Constants.EnableHelpLabels;
        }

        private static void OnNewChatLogEntry(object sender, ChatLogEntryEvent chatLogEntryEvent)
        {
            // delegate event from chat log, not required to subsribe
            // this updates 100 times a second and only sends a line when it gets a new one
            if (sender == null)
            {
                return;
            }
            var chatLogEntry = chatLogEntryEvent.ChatLogEntry;
            try
            {
                LogPublisher.Process(chatLogEntry);
            }
            catch (Exception ex)
            {
            }
        }

        private static void OnNewMonsterEntries(object sender, ActorEntitiesEvent actorEntitiesEvent)
        {
            // delegate event from monster entities from ram, not required to subsribe
            // this updates 10x a second and only sends data if the items are found in ram
            // currently there no change/new/removed event handling (looking into it)
            if (sender == null)
            {
                return;
            }
            var monsterEntities = actorEntitiesEvent.ActorEntities;

            if (Analyzer.Instance.PlayerId > 0 && monsterEntities.Any())
            {
                //var targetingMe = monsterEntities.Where(x => x.TargetID == Analyzer.Instance.PlayerId).ToList();
                //var targetingMe = monsterEntities.Where(x => x.Name == "Dung Midge Swarm" && x.TargetID != 0).ToList();
                //if (targetingMe.Any())
                //{
                //    LogHelper.Log(Logger, "Monster Name:" + targetingMe.First().Name + " ID:" + targetingMe.First().OwnerID + " Target:" + targetingMe.First().TargetID + " FateId:" + targetingMe.First().Fate + " MapId:" + targetingMe.First().MapIndex + "IsValid:" + targetingMe.First().IsValid + " Job:" + targetingMe.First().Job + " Type:" + targetingMe.First().Type + " NPCID1:" + targetingMe.First().NPCID1 + " NPCID2:" + targetingMe.First().NPCID2 + " Owner:" + targetingMe.First().OwnerID, LogLevel.Trace);
                //}

                var targetingMe = monsterEntities.Where(x => x.TargetID == Analyzer.Instance.PlayerId && x.IsValid && x.OwnerID == 3758096384)
                                                 .ToList();
                if (Analyzer.Instance.EncounterId < 1 && targetingMe.Any())
                {
                    LogHelper.Log(Logger, "Monster Name:" + targetingMe.First()
                                                                       .Name + " ID:" + targetingMe.First()
                                                                                                   .OwnerID + " Target:" + targetingMe.First()
                                                                                                                                      .TargetID + " FateId:" + targetingMe.First()
                                                                                                                                                                          .Fate + " MapId:" + targetingMe.First()
                                                                                                                                                                                                         .MapIndex + "IsValid:" + targetingMe.First()
                                                                                                                                                                                                                                             .IsValid + " Job:" + targetingMe.First()
                                                                                                                                                                                                                                                                             .Job + " Type:" + targetingMe.First()
                                                                                                                                                                                                                                                                                                          .Type + " NPCID1:" + targetingMe.First()
                                                                                                                                                                                                                                                                                                                                          .NPCID1 + " NPCID2:" + targetingMe.First()
                                                                                                                                                                                                                                                                                                                                                                            .NPCID2 + " Owner:" + targetingMe.First()
                                                                                                                                                                                                                                                                                                                                                                                                             .OwnerID, LogLevel.Trace);
                    Analyzer.Instance.StartEncounter(targetingMe.First()
                                                                .MapIndex);
                }
                else if (Analyzer.Instance.EncounterId > 0 && !targetingMe.Any())
                {
                    Analyzer.Instance.EndEncounter();
                }
            }
        }

        private static void OnNewNPCEntries(object sender, ActorEntitiesEvent actorEntitiesEvent)
        {
            // delegate event from npc entities from ram, not required to subsribe
            // this list includes anything that is not a player or monster
            // this updates 10x a second and only sends data if the items are found in ram
            // currently there no change/new/removed event handling (looking into it)
            if (sender == null)
            {
                return;
            }
            var npcEntities = actorEntitiesEvent.ActorEntities;
        }

        private static void OnNewPCEntries(object sender, ActorEntitiesEvent actorEntitiesEvent)
        {
            // delegate event from player entities from ram, not required to subsribe
            // this updates 10x a second and only sends data if the items are found in ram
            // currently there no change/new/removed event handling (looking into it)
            if (sender == null)
            {
                return;
            }
            var pcEntities = actorEntitiesEvent.ActorEntities;

            if (pcEntities.Any())
            {
                Analyzer.Instance.PlayerId = pcEntities.First()
                                                       .ID;
            }
        }

        private static void OnNewPlayerEntity(object sender, PlayerEntityEvent playerEntityEvent)
        {
            // delegate event from player info from ram, not required to subsribe
            // this is for YOU and includes all your stats and your agro list with hate values as %
            // this updates 10x a second and only sends data when the newly read data is differen than what was previously sent
            if (sender == null)
            {
                return;
            }
            var playerEntity = playerEntityEvent.PlayerEntity;
        }

        private static void OnNewTargetEntity(object sender, TargetEntityEvent targetEntityEvent)
        {
            // delegate event from target info from ram, not required to subsribe
            // this includes the full entities for current, previous, mouseover, focus targets (if 0+ are found)
            // it also includes a list of upto 16 targets that currently have hate on the currently targeted monster
            // these hate values are realtime and change based on the action used
            // this updates 10x a second and only sends data when the newly read data is differen than what was previously sent
            if (sender == null)
            {
                return;
            }
            var targetEntity = targetEntityEvent.TargetEntity;
        }

        private static void OnNewParseEntity(object sender, ParseEntityEvent parseEntityEvent)
        {
            // delegate event from data work; which right now has basic parsing stats for widgets.
            // includes global total stats for damage, healing, damage taken
            // include player list with name, hps, dps, dtps, total stats like the global and percent of each total stat
            if (sender == null)
            {
                return;
            }
            var parseEntity = parseEntityEvent.ParseEntity;
        }

        private static void OnNewPartyEntries(object sender, PartyEntitiesEvent partyEntitiesEvent)
        {
            // delegate event from party info worker that will give basic info on party members
            if (sender == null)
            {
                return;
            }
            var partyEntities = partyEntitiesEvent.PartyEntities;

            if (Analyzer.Instance.EncounterId > 0 && Analyzer.Instance.PartyCaptured == false)
            {
                Analyzer.Instance.PartyCaptured = true;
                foreach (var partyEntity in partyEntities)
                {
                    Analyzer.Instance.CapturePartyMember(partyEntity);
                }
            }
            else if (Analyzer.Instance.EncounterId > 0)
            {
                foreach (var partyEntity in partyEntities)
                {
                    // Kind of a shitty way but whatever

                    var tempEntries = Analyzer.Instance.partyMemberStatusEntries.Where(x => x.PartyMemberId == partyEntity.ID)
                                              .ToList();

                    foreach (var statusEntry in partyEntity.StatusEntries)
                    {
                        if (tempEntries.RemoveAll(x => x.CasterId == statusEntry.CasterID && x.StatusId == statusEntry.StatusID && x.TargetName == statusEntry.TargetName) == 0)
                        {
                            // Nothing removed - new status effect
                            Analyzer.Instance.NewPartyMemberStatusEntry(statusEntry, partyEntity.ID);
                        }
                    }

                    // Now the only entries remaining in tempEntries should be expired:
                    foreach (var partyMemberStatusEntry in tempEntries)
                    {
                        Analyzer.Instance.EndPartyMemberStatusEntry(partyMemberStatusEntry);
                    }
                }
            }
        }

        #endregion
    }
}
