using HarmonyLib;
using System;
using System.Collections.Generic;
using JumpKing.MiscSystems.LocationText;
using JumpKing;
using JumpKing.MiscSystems.Achievements;

namespace InputRecorder.States;
public class ScreenState
{
    private HashSet<int> visitedScreens = new();
    private Dictionary<int, string> unlockLocations = new();

    public ScreenState() {
        Type locationTextManagerType = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationTextManager");
        LocationSettings settings = (LocationSettings)AccessTools.Property(locationTextManagerType, "SETTINGS").GetValue(null);
        foreach (Location location in settings.locations) {
            unlockLocations[location.unlock] = location.name;
        }

        Type achievementManagerType = AccessTools.TypeByName("JumpKing.MiscSystems.Achievements.AchievementManager");
    }
    public bool isNewScreen() {
        return visitedScreens.Add(Camera.CurrentScreenIndex1);
    }
    public string ToTasString() {
        PlayerStats stats = InputRecorder.GetCurrentStats();
        int hours = (int)stats.timeSpan.TotalHours;
        int minutes = stats.timeSpan.Minutes;
        int seconds = stats.timeSpan.Seconds;
        int milliseconds = stats.timeSpan.Milliseconds;
        string timestamp = hours.ToString("00") + ":" + minutes.ToString("00") + ":" 
            + seconds.ToString("00") + "." + milliseconds.ToString("000");

        string result = string.Empty;
        if (unlockLocations.ContainsKey(Camera.CurrentScreenIndex1)) {
            result += $"#{unlockLocations[Camera.CurrentScreenIndex1]}" + Environment.NewLine;
        }
        result += $"  #Screen {Camera.CurrentScreenIndex1} {timestamp}({stats._ticks})";
        return result;
    }
}