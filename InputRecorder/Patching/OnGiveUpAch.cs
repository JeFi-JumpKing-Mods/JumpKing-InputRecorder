using HarmonyLib;
using System;
using System.Reflection;
using InputRecorder.States;

namespace InputRecorder.Patching;

public class OnGiveUpAch
{
    public OnGiveUpAch(Harmony harmony)
    {
        Type type = AccessTools.TypeByName("JumpKing.MiscSystems.Achievements.OnGiveUpAch");
        MethodInfo MyRun = AccessTools.Method(type, "MyRun");
        harmony.Patch(
            MyRun,
            postfix: new HarmonyMethod(AccessTools.Method(typeof(OnGiveUpAch), nameof(postMyRun)))
        );
    }
    private static void postMyRun() {
        if (InputRecorder.Prefs.IsEnableRecording) {
            StateManager.EndingMessage = "#Give up";
            StateManager.isGiveUp = true;
        }
    }
}