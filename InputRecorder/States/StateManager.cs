using HarmonyLib;
using JumpKing.Controller;
using JumpKing.Player;
using BehaviorTree;
using JumpKing.MiscSystems.Achievements;
using JumpKing.GameManager;
using JumpKing;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System;
using JumpKing.MiscEntities.WorldItems.Inventory;
using JumpKing.MiscEntities.WorldItems;

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
    public static bool isPaused = false;
    public static bool isLastPaused = false;
    public static bool isOnGround = false;

    public static string EndingMessage;

    public static void Initialize() {
        MapName = GetMapName();
        InitialStats = GetCurrentStats();
        player = GameLoop.m_player;
    }
    
    public static void StartRecording() {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = $"{timestamp}_{Sanitize(MapName)}_A{InitialStats.attempts}-S{InitialStats.session}.tas";
        string filePath = Path.Combine(InputRecorder.AssemblyPath, InputRecorder.TAS_FOLDER, fileName);

        writer = new TasWriter(filePath);
        writer.WriteLineQueued($"#{MapName} ({InitialStats.steam_level_id}), attempts:{InitialStats.attempts}, sessions:{InitialStats.session}");
        writer.WriteLineQueued($"#position={player.m_body.Position}, velocity={player.m_body.Velocity}");
        writer.WriteLineQueued($"#Ring={InventoryManager.HasItemEnabled(Items.SnakeRing)}, Boots={InventoryManager.HasItemEnabled(Items.GiantBoots)}");
        

        screenState = new ScreenState();
        currentTasState = new TasState(new PadState(), 0);

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
        if (isOnGround && !screenState.isPreviousScreen()) {
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
        currentTasState = null;
        if (EndingMessage!=string.Empty) {
            writer.WriteLineQueued(EndingMessage);
        }
        writer.CloseQueued();
        writer.Dispose();


        player = null;
    }

    public static void Terminate() {
        MapName = string.Empty;
        InitialStats = new();
        player = null;
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