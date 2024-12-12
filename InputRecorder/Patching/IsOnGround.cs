// using HarmonyLib;
// using System.Reflection;
// using JK = JumpKing.Player;
// using BehaviorTree;
// using InputRecorder.States;

// namespace InputRecorder.Patching;

// public class IsOnGround
// {
//     public IsOnGround(Harmony harmony)
//     {
//         MethodInfo MyRun = typeof(JK.IsOnGround).GetMethod("MyRun", BindingFlags.Instance | BindingFlags.NonPublic);
//         harmony.Patch(
//             MyRun,
//             postfix: new HarmonyMethod(AccessTools.Method(typeof(IsOnGround), nameof(postMyRun)))
//         );
//     }
//     private static void postMyRun(BTresult __result) {
//         StateManager.isOnGround = (__result==BTresult.Success);
//     }
// }