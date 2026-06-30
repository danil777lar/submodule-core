using System;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

public class OverlayEntryDebug : OverlayEntry
{
    public override string Group => OverlayEntry.GROUP_DEBUG;
    public override Func<string> GetData => GenerateData;

    private string GenerateData()
    {
        string info = "";

        foreach (string g in LarjeDebug.Overlay.GetGroups())
        {
            if (LarjeDebug.Overlay.IsDebugGroupEnabled(g))
            {
                info += $"[{g}]\n";
                var messages = LarjeDebug.Overlay.GetLogMessages();
                if (messages.TryGetValue(g, out var groupMessages))
                {
                    foreach (string message in groupMessages)
                    {
                        info += $"    {message}\n";
                    }
                }
            }
        }

        return info;
    }
}

namespace LarjeDebug
{
    public static class Overlay
    {
        private static IDataService _dataService;
        private static Dictionary<string, List<string>> logMessagesByGroup = new Dictionary<string, List<string>>();

        private static IDataService DataService
        {
            get
            {
                if (_dataService == null)
                {
                    _dataService = DIContainer.GetService<IDataService>();
                }
                return _dataService;
            }
        }

        public static string[] GetGroups()
        {
            return new List<string>(logMessagesByGroup.Keys).ToArray();
        }

        public static void Log(string group, string message)
        {
            if (!logMessagesByGroup.ContainsKey(group))
            {
                logMessagesByGroup[group] = new List<string>();
            }

            logMessagesByGroup[group].Add(message);

            if (logMessagesByGroup[group].Count > 100)
            {
                logMessagesByGroup[group].RemoveAt(0);
            }
        }

        public static Dictionary<string, List<string>> GetLogMessages()
        {
            Dictionary<string, List<string>> logMessagesByGroupCopy = new Dictionary<string, List<string>>();
            foreach (var kvp in logMessagesByGroup)
            {
                logMessagesByGroupCopy[kvp.Key] = new List<string>(kvp.Value);
            }
            logMessagesByGroup.Clear();
            return logMessagesByGroupCopy;
        }

        public static bool IsDebugGroupEnabled(string g)
        {
            if (DataService.SystemData != null)
            {
                return !DataService.SystemData.IternalData.DebugConsoleData.hiddenDebugGroups.Contains(g);
            }
            return true;
        }

        public static void SetDebugGroupEnabled(string g, bool enabled)
        {
            if (DataService.SystemData != null)
            {
                if (enabled && DataService.SystemData.IternalData.DebugConsoleData.hiddenDebugGroups.Contains(g))
                {
                    DataService.SystemData.IternalData.DebugConsoleData.hiddenDebugGroups.Remove(g);
                }
                else if (!enabled && !DataService.SystemData.IternalData.DebugConsoleData.hiddenDebugGroups.Contains(g))
                {
                    DataService.SystemData.IternalData.DebugConsoleData.hiddenDebugGroups.Add(g);
                }
            }
        }
    }
}
