using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;

namespace Kaze.UnusedIDsChecker
{
    public class UnusedIDsCheckerPlugin : RocketPlugin
    {
        protected override void Load()
        {
            Level.onPostLevelLoaded += OnPostLevelLoaded;
            Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} by kaze has been loaded! <3");
        }

        protected override void Unload()
        {
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
            Logger.Log($"{Name} by kaze has been unloaded! <3");
        }

        private void OnPostLevelLoaded(int level)
        {
            Logger.Log("Level loaded! Starting unused ID check...");

            List<(ushort start, ushort end)> ranges = new List<(ushort, ushort)>();

            ushort? rangeStart = null;
            ushort? lastID = null;

            for (ushort id = 1923; id < ushort.MaxValue; id++)
            {
                var asset = Assets.find(EAssetType.ITEM, id);
                if (asset == null)
                {
                    if (rangeStart == null)
                    {
                        rangeStart = id;
                        lastID = id;
                    }
                    else
                        lastID = id;
                    
                }
                else if (rangeStart != null)
                {
                    ranges.Add((rangeStart.Value, lastID.Value));
                    rangeStart = null;
                    lastID = null;
                }
            }

            if (rangeStart != null)
                ranges.Add((rangeStart.Value, lastID.Value));

            foreach (var (start, end) in ranges)
            {
                if (start == end)
                    Logger.LogWarning($"Unused ID: {start}");
                else
                    Logger.LogWarning($"Unused ID range: {start} -> {end}");
            }
            Logger.Log($"Unused ID check complete.");
        }
    }
}
