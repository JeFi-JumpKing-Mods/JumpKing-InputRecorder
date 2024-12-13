using HarmonyLib;
using JumpKing.Controller;
using JumpKing.Player;
using BehaviorTree;
using JumpKing.MiscSystems.Achievements;

namespace InputRecorder.States;
public static class StateManager
{
    private static PlayerEntity player;
    private static TasWriter writer;
    private static TasState currentTasState;
    private static ScreenState screenState;
    public static PadState PadState;
    public static PadState PressedPadState;
    public static bool isPaused = false;
    public static bool isLastPaused = false;
    public static bool isOnGround = false;
    public static string EndingMessage;
    
    public static void StartRecording(string filePath, PlayerEntity p_player) {
        player = p_player;
        writer = new TasWriter(filePath);
        writer.WriteLineQueued($"#{InputRecorder.MapName} ({InputRecorder.CurrentStats.steam_level_id}), attempts:{InputRecorder.CurrentStats.attempts}, sessions:{InputRecorder.CurrentStats.session}");
        writer.WriteLineQueued($"#position={player.m_body.Position}, velocity={player.m_body.Velocity}");

        screenState = new ScreenState();
        currentTasState = new TasState(PadState, PressedPadState);

        EndingMessage = "";
    }
    public static void Update() {
        if (player==null) {
            return;
        }

        PadState = ControllerManager.instance.GetPadState();
        PressedPadState = ControllerManager.instance.GetPressedPadState();
        isOnGround = (Traverse.Create(player).Field("m_is_on_ground_state").Field("m_last_result").GetValue<BTresult>() == BTresult.Success);
        // isLastPaused = isPaused;
        // isPaused = Traverse.Create(AccessTools.TypeByName("JumpKing.PauseMenu.PauseManager")).Field("instance").Field("_paused").GetValue<bool>();
    }

    public static void WriteTAS() {
        if (isOnGround && screenState.isNewScreen()) {
            writer.WriteLineQueued(screenState.ToTasString());
        }

        TasState newTasState = (true) ? new TasState(PadState) : new TasState(PadState);
        if (!currentTasState.Extends(newTasState)) {
            writer.WriteLineQueued(currentTasState.ToTasString());
            currentTasState = newTasState;
        }
    }

    public static void Draw() {
        return;
    }
    
    public static void EndRecording() {
        writer.WriteLineQueued(currentTasState.ToTasString());
        if (EndingMessage!=string.Empty) {
            writer.WriteLineQueued(EndingMessage);
        }
        writer.CloseQueued();
        writer.Dispose();

        player = null;
    }
}