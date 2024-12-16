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
    private int previousScreen = -273;

    public ScreenState() {
        LocationSettings settings = (LocationSettings)AccessTools.Property(
            "JumpKing.MiscSystems.LocationText.LocationTextManager:SETTINGS").GetValue(null);
        foreach (Location location in settings.locations) {
            unlockLocations[location.unlock] = location.name;
        }
    }
    public bool isPreviousScreen() {
        return previousScreen == (previousScreen=Camera.CurrentScreenIndex1);
    }
    public bool isNewScreen() {
        return visitedScreens.Add(Camera.CurrentScreenIndex1);
    }
    public string ToTasString() {
        string result = string.Empty;
        if (unlockLocations.ContainsKey(Camera.CurrentScreenIndex1)) {
            result += $"#{unlockLocations[Camera.CurrentScreenIndex1]}" + Environment.NewLine;
            unlockLocations.Remove(Camera.CurrentScreenIndex1);
        }

        result += $" #Screen-{Camera.CurrentScreenIndex1}";
        if (isNewScreen()) {
            PlayerStats stats = StateManager.GetCurrentStats();
            int hours = (int)stats.timeSpan.TotalHours;
            int minutes = stats.timeSpan.Minutes;
            int seconds = stats.timeSpan.Seconds;
            int milliseconds = stats.timeSpan.Milliseconds;
            string timestamp = hours.ToString("00") + ":" + minutes.ToString("00") + ":" 
                + seconds.ToString("00") + "." + milliseconds.ToString("000");
            result += $" {timestamp}({stats._ticks})";
        }
        return result;
    }
}