// using HarmonyLib;
// using InputRecorder.States;
// using System.Reflection;
// using JK = JumpKing.Controller;

// namespace InputRecorder.Patching;

// public class ControllerManager
// {
//     public ControllerManager(Harmony harmony)
//     {
//         MethodInfo Update = typeof(JK.ControllerManager).GetMethod(nameof(JK.ControllerManager.Update));
//         harmony.Patch(
//             Update,
//             postfix: new HarmonyMethod(AccessTools.Method(typeof(ControllerManager), nameof(postUpdate)))
//         );
//     }
//     private static void postUpdate(JK.PadState __result) {
//         StateManager.PadState = __result;
//     }
// }