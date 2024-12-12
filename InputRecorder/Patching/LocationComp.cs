// using HarmonyLib;
// using System;
// using System.Reflection;

// namespace InputRecorder.Patching;

// public class LocationComp
// {
//     public LocationComp(Harmony harmony)
//     {
//         Type type = Type.GetType("JumpKing.MiscSystems.LocationText.LocationComp, JumpKing");
//         MethodInfo PollCurrent = type.GetMethod("PollCurrent", BindingFlags.Instance | BindingFlags.Public);
//         harmony.Patch(
//             PollCurrent,
//             postfix: new HarmonyMethod(AccessTools.Method(typeof(LocationComp), nameof(postPollCurrent)))
//         );

//         MethodInfo PollNewScreen = type.GetMethod("PollNewScreen", BindingFlags.Instance | BindingFlags.Public);
//         harmony.Patch(
//             PollNewScreen,
//             postfix: new HarmonyMethod(AccessTools.Method(typeof(LocationComp), nameof(postPollNewScreen)))
//         );
//     }
//     private static void postPollCurrent(ref string out_location_name) {
//     }
//     private static void postPollNewScreen(ref string location_name) {
//     }
// }