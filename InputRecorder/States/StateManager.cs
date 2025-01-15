using HarmonyLib;

using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using BehaviorTree;
using JumpKing;
using JumpKing.Controller;
using JumpKing.GameManager;
using JumpKing.MiscEntities.WorldItems;
using JumpKing.MiscEntities.WorldItems.Inventory;
using JumpKing.MiscSystems.Achievements;
using JumpKing.Player;

namespace InputRecorder.States;
public static class StateManager
{
    private static TasWriter writer;
    private static TasState currentTasState;
    private static ScreenState screenState;

    private static PlayerEntity player;
    public static string MapName = string.Empty;
    public static PlayerStats InitialStats;

    public static PadState PadState;
    public static PadState PressedPadState;
    private static PadState SimplifiedPadState {
        get {
            if (!isLastPaused && PressedPadState.restart) {
                return new PadState{restart = true};
            }
            PadState result = new();
            if (!isPaused) {
                if (isOnGround == BTresult.Success) {
                    jumpLeniency = 2;
                    if (failState==BTresult.Failure) {
                        // When FailState fail (not splat), determine player is charging, walking or idling by input.
                        result.left = PadState.left;
                        result.right = PadState.right;
                        result.jump = PadState.jump && jumpState == BTresult.Running;
                    }
                    else if (failState==BTresult.Success) {
                        // When FailState Success, FailState.MyRun() will accept left, right, jump input.
                        result.left = PadState.left;
                        result.right = PadState.right;
                        result.jump = PadState.jump;
                    }
                }
                else if (jumpLeniency>0) {
                    // JumpState has leniency (2f) when not on ground
                    jumpLeniency--;
                    result.jump = PadState.jump && jumpState == BTresult.Running;
                }
                if (jumpLeniency>=0 && jumpState == BTresult.Success) {
                    // At first frame of jumpState == BTresult.Success, 
                    // left & right still can affect jump direction.
                    jumpLeniency = -1;
                    result.left = PadState.left;
                    result.right = PadState.right;
                }
                // left and right will be cancel out in InputComponent
                if (result.left == result.right) {
                    result.left = false;
                    result.right = false;
                }
            }
            else {
                result.up = PressedPadState.up;
                result.down = PressedPadState.down;
                result.left = PressedPadState.left;
                result.right = PressedPadState.right;
                result.confirm = PressedPadState.confirm;
                result.cancel = PressedPadState.cancel;
            }
            if (isPaused != isLastPaused) {
                result.pause = true;
            }
            if (!isLastPaused) {
                result.boots = PressedPadState.boots;
                result.snake = PressedPadState.snake;
            }
            return result;
        }
    }
    private static Traverse _isOnGround;
    private static BTresult isOnGround;
    private static Traverse _jumpState;
    private static BTresult jumpState = BTresult.NULL;
    private static Traverse _failState;
    private static int jumpLeniency = 0;
    private static BTresult failState = BTresult.NULL;
    private static Traverse _isPaused;
    private static bool isPaused = false;
    private static bool isLastPaused = false;
    public static bool isGiveUp = false;


    public static string EndingMessage;

    public static void Initialize() {
        MapName = GetMapName();
        InitialStats = GetCurrentStats();
        player = GameLoop.m_player;
        _isOnGround = Traverse.Create(player).Field("m_is_on_ground_state").Field("m_last_result");
        _jumpState = Traverse.Create(player).Field("m_jump_state").Field("m_last_result");
        _failState = Traverse.Create(player).Field("m_fail_state").Field("m_last_result");
        _isPaused = Traverse.Create(AccessTools.TypeByName("JumpKing.PauseMenu.PauseManager")).Field("instance").Field("_paused");

        isOnGround = BTresult.NULL;
        jumpState = BTresult.NULL;
        jumpLeniency = 0;
        failState = BTresult.NULL;
        isPaused = false;
        isLastPaused = false;
        isGiveUp = false;

    }
    public static void StartRecording() {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = $"{timestamp}_{Sanitize(MapName)}_A{InitialStats.attempts}-S{InitialStats.session}.tas";
        string filePath = Path.Combine(InputRecorder.AssemblyPath, InputRecorder.TAS_FOLDER, fileName);

        writer = new TasWriter(filePath);
        writer.WriteLineQueued($"#{MapName} ({InitialStats.steam_level_id}), attempts:{InitialStats.attempts}, sessions:{InitialStats.session}");
        writer.WriteLineQueued($"#Position={player.m_body.Position}, Velocity={player.m_body.Velocity}");
        writer.WriteLineQueued($"#Ring={InventoryManager.HasItemEnabled(Items.SnakeRing)}, Boots={InventoryManager.HasItemEnabled(Items.GiantBoots)}");
        

        screenState = new ScreenState();
        currentTasState = new TasState(new PadState(), 0);

        EndingMessage = "";
    }
    // StateManager.Update() is called at the end of JumpGame.Update(), 
    // so all game elements have been updated before.
    public static void Update() {
        if (player==null) {
            return;
        }

        PadState = ControllerManager.instance.GetPadState();
        PressedPadState = ControllerManager.instance.GetPressedPadState();
        isOnGround = _isOnGround.GetValue<BTresult>();
        jumpState = _jumpState.GetValue<BTresult>();
        failState = _failState.GetValue<BTresult>();
        isLastPaused = isPaused;
        isPaused = _isPaused.GetValue<bool>();
    }
    public static void WriteTAS() {
        if (isOnGround == BTresult.Success && !screenState.isPreviousScreen()) {
            writer.WriteLineQueued(screenState.ToTasString());
        }

        TasState newTasState = InputRecorder.Prefs.IsSimplifyInput ? new TasState(SimplifiedPadState) : new TasState(PadState);
        if (!currentTasState.Extends(newTasState)) {
            writer.WriteLineQueued(currentTasState.ToTasString());
            currentTasState = newTasState;
        }
    }
    public static void Draw() {
        return;
    }
    public static void EndRecording() {
        Update();
        WriteTAS();
        writer.WriteLineQueued(currentTasState.ToTasString());
        if (EndingMessage!=string.Empty) {
            writer.WriteLineQueued(EndingMessage);
        }
        writer.FlushQueued();
        writer.CloseQueued();
        writer.Dispose();

        currentTasState = null;
        player = null;
    }
    public static void Terminate() {
        MapName = string.Empty;
        InitialStats = new();
        player = null;

        _isOnGround = new Traverse(false);
        _isPaused = new Traverse(BTresult.Failure);
        _jumpState = new Traverse(BTresult.Failure);
    }

    public static PlayerStats GetCurrentStats() {
        object achievementManagerinstance = AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:instance").GetValue(null);
        MethodInfo GetCurrentStats = AccessTools.Method("JumpKing.MiscSystems.Achievements.AchievementManager:GetCurrentStats");
        return (PlayerStats)GetCurrentStats.Invoke(achievementManagerinstance, null);
    }
    private static string GetMapName() {
        JKContentManager contentManager = Game1.instance.contentManager;
        if (contentManager == null)
        {
            return "Debug";
        }
        if (contentManager.level != null)
        {
            return contentManager.level.Name;
        }
        return "Nexile Maps";
    }
    private static string Sanitize(string name)
    {
        name = name.Trim();
        if (name == string.Empty)
        {
            name = "-";
        }
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '#');
        }
        foreach (char c in Path.GetInvalidPathChars())
        {
            name = name.Replace(c, '#');
        }
        name = Regex.Replace(name, "^\\.\\.$", "-", RegexOptions.IgnoreCase);
        name = Regex.Replace(name, "^(con|prn|aux|nul|com\\d|lpt\\d)$", $"-", RegexOptions.IgnoreCase);

        return name;
    }
}