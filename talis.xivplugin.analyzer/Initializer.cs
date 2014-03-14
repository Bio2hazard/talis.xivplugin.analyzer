// Talis.XIVPlugin.Analyzer
// Initializer.cs

using System;
using System.Xml.Linq;
using Talis.XIVPlugin.Analyzer.Properties;

namespace Talis.XIVPlugin.Analyzer
{
    internal static class Initializer
    {
        #region Declarations

        #endregion

        /// <summary>
        /// </summary>
        public static void LoadSettings()
        {
            if (Constants.XSettings != null)
            {
                Settings.Default.Reset();
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Setting"))
                {
                    var xKey = (string) xElement.Attribute("Key");
                    var xValue = (string) xElement.Element("Value");
                    if (String.IsNullOrWhiteSpace(xKey) || String.IsNullOrWhiteSpace(xValue))
                    {
                        return;
                    }
                    if (Constants.Settings.Contains(xKey))
                    {
                        Settings.Default.SetValue(xKey, xValue);
                    }
                    else
                    {
                        Constants.Settings.Add(xKey);
                    }
                }
            }
        }
    }
}
