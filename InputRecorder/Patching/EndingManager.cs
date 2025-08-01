using HarmonyLib;
using System;
using System.Reflection;
using JumpKing.MiscSystems.Achievements;
using InputRecorder.States;

namespace InputRecorder.Patching;

public class EndingManager
{
    public EndingManager(Harmony harmony)
    {
        Type type = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.EndingManager");
        MethodInfo CheckWin = type.GetMethod("CheckWin");
        harmony.Patch(
            CheckWin,
            postfix: new HarmonyMethod(AccessTools.Method(typeof(EndingManager), nameof(postCheckWin)))
        );
    }
    private static void postCheckWin(bool __result) {
        if (__result) {
            PlayerStats stats = StateManager.GetCurrentStats();
            int hours = (int)stats.timeSpan.TotalHours;
            int minutes = stats.timeSpan.Minutes;
            int seconds = stats.timeSpan.Seconds;
            int milliseconds = stats.timeSpan.Milliseconds;
            string timestamp = hours.ToString("00") + ":" + minutes.ToString("00") + ":" 
                + seconds.ToString("00") + "." + milliseconds.ToString("000");

            StateManager.EndingMessage = $"#Win {timestamp}({stats._ticks})";
        }
    }
}