// Talis.XIVPlugin.Analyzer
// LogPublisher.cs

using System;
using FFXIVAPP.Common.Core.Memory;
using NLog;
using Talis.XIVPlugin.Analyzer.Helpers;

namespace Talis.XIVPlugin.Analyzer.Utilities
{
    public static class LogPublisher
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

        public static void Process(ChatLogEntry chatEntry)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }
    }
}
