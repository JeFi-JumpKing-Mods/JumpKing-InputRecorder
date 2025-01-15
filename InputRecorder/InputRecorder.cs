using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using JumpKing;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using Microsoft.Xna.Framework;

using InputRecorder.Menu;
using InputRecorder.Nodes;
using InputRecorder.States;

namespace InputRecorder;

[JumpKingMod(IDENTIFIER)]
public static class InputRecorder
{
    const string IDENTIFIER = "JeFi.InputRecorder";
    const string HARMONY_IDENTIFIER = "JeFi.InputRecorder.Harmony";
    const string SETTINGS_FILE = "JeFi.InputRecorder.Preferences.xml";
    const string BINDING_FILE = "TasBinding.xml";
    public const string TAS_FOLDER = "TAS";

    public static string AssemblyPath { get; set; }
    public static Preferences Prefs { get; private set; }
    public static TasBinding TasBinding { get; private set; }
    
    [BeforeLevelLoad]
    public static void BeforeLevelLoad()
    {
        AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
        Debugger.Launch();
        Harmony.DEBUG = true;
        Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", $@"{AssemblyPath}\harmony.log.txt");
#endif
        if (!Directory.Exists(Path.Combine(AssemblyPath, TAS_FOLDER)))
        {
            Directory.CreateDirectory(Path.Combine(AssemblyPath, TAS_FOLDER));
        }

        try
        {
            Prefs = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            Prefs = new Preferences();
        }
        Prefs.PropertyChanged += SaveSettingsOnFile;
        // Ture-off TAS recording if TurnoffInputRecordingEverytime=true
        if (Prefs.TurnoffInputRecordingEverytime) {
            Prefs.IsEnableRecording = false;
        }

        try
        {
            TasBinding = XmlSerializerHelper.Deserialize<TasBinding>($@"{AssemblyPath}\{BINDING_FILE}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            TasBinding = TasBinding.GetDefault();
            XmlSerializerHelper.Serialize($@"{AssemblyPath}\{BINDING_FILE}", TasBinding);
        }

        Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

        new Patching.JumpGame(harmony);
        new Patching.EndingManager(harmony);
        new Patching.OnGiveUpAch(harmony);

#if DEBUG
        Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", null);
#endif
    }

    [OnLevelStart]
    public static void OnLevelStart() {
        if (!Directory.Exists(Path.Combine(AssemblyPath, TAS_FOLDER)))
        {
            Directory.CreateDirectory(Path.Combine(AssemblyPath, TAS_FOLDER));
        }

        StateManager.Initialize();
        if (Prefs.IsEnableRecording) {
            StateManager.StartRecording();
        }
    }

    [OnLevelEnd]
    public static void OnLevelEnd() {
        if (Prefs.IsEnableRecording) {
            if (StateManager.EndingMessage == string.Empty) {
                StateManager.EndingMessage = 
                    Game1.instance.m_game.m_restart_state ? "#Restart" : "#Exit to Menu";
            }
            StateManager.EndRecording();
        }
        StateManager.Terminate();
    }

    #region Menu Items
    [MainMenuItemSetting]
    public static ToggleRecording ToggleRecording(object factory, GuiFormat format)
    {
        return new ToggleRecording();
    }

    [MainMenuItemSetting]
    [PauseMenuItemSetting]
    public static ToggleSimplifyInput ToggleSimplifyInput(object factory, GuiFormat format)
    {
        return new ToggleSimplifyInput();
    }

    [MainMenuItemSetting]
    [PauseMenuItemSetting]
    public static ExplorerTextButton OpenFolderExplorer(object factory, GuiFormat format)
    {
        return new ExplorerTextButton("Open Input Files Folder", new NodeOpenFolderExplorer(AssemblyPath), Color.Lime);
    }

    #endregion

    private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        try
        {
            XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Prefs);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
        }
    }
}
