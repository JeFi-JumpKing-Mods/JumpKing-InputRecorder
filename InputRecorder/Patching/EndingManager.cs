using HarmonyLib;
using InputRecorder.States;
using JumpKing.MiscSystems.Achievements;
using System;
using System.Reflection;

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
            PlayerStats stats = InputRecorder.GetCurrentStats();
            int hours = (int)stats.timeSpan.TotalHours;
            int minutes = stats.timeSpan.Minutes;
            int seconds = stats.timeSpan.Seconds;
            int milliseconds = stats.timeSpan.Milliseconds;
            string timestamp = hours.ToString("00") + ":" + minutes.ToString("00") + ":" 
                + seconds.ToString("00") + "." + milliseconds.ToString("000");

            StateManager.EndingMessage = $"#End {timestamp}({stats._ticks})";
        }
    }
}