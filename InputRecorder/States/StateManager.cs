using System.Collections.Generic;
using HarmonyLib;
using JumpKing.Controller;
using JumpKing.Player;
using JumpKing.GameManager;
using BehaviorTree;

namespace InputRecorder.States;
public static class StateManager
{
    private static PlayerEntity player;
    private static TasWriter writer;
    private static TasState currentTasState;
    public static PadState PadState;
    public static PadState PressedPadState;
    public static bool isPaused = false;
    public static bool isLastPaused = false;
    public static bool isOnGround = false;
    
    public static void StartRecording(string filePath, PlayerEntity p_player) {
        player = p_player;
        writer = new TasWriter(filePath);
        // writer.WriteLineQueued("");

        currentTasState = new TasState(PadState, PressedPadState);
    }
    public static void Update() {
        if (player==null) {
            return;
        }

        PadState = ControllerManager.instance.GetPadState();
        PressedPadState = ControllerManager.instance.GetPressedPadState();

        // isOnGround = (Traverse.Create(player).Field("m_is_on_ground_state").Field("m_last_result").GetValue<BTresult>() == BTresult.Success);
        // isLastPaused = isPaused;
        // isPaused = Traverse.Create(AccessTools.TypeByName("JumpKing.PauseMenu.PauseManager")).Field("instance").Field("_paused").GetValue<bool>();
    }

    public static void WriteTAS() {
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
        writer.CloseQueued();
        writer.Dispose();

        player = null;
    }
}