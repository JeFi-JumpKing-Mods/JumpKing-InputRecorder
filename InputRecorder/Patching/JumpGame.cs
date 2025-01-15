using HarmonyLib;
using System;
using System.Reflection;
using JK = JumpKing;
using JumpKing.GameManager;
using InputRecorder.States;

namespace InputRecorder.Patching;

public class JumpGame
{
    public JumpGame(Harmony harmony)
    {
        Type type = typeof(JK.JumpGame);
        MethodInfo Update = type.GetMethod(nameof(JK.JumpGame.Update));
        harmony.Patch(
            Update,
            postfix: new HarmonyMethod(AccessTools.Method(typeof(JumpGame), nameof(postUpdate)))
        );
    }
    private static void postUpdate() {
        if (GameLoop.instance.IsRunning()) {
            StateManager.Update();
            if (InputRecorder.Prefs.IsEnableRecording) {
                StateManager.WriteTAS();
            }
            if (StateManager.isGiveUp) {
                StateManager.EndRecording();
            }
        }
    }
}