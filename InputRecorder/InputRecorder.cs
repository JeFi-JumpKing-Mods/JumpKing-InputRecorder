using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using JumpKing.Mods;
using JumpKing.PauseMenu;
using InputRecorder.States;

using InputRecorder.Menu;

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
    public static Preferences Preferences { get; private set; }
    public static TasBinding TasBinding { get; private set; }

    public static bool IsEnabledRecording = false;

    [BeforeLevelLoad]
    public static void BeforeLevelLoad()
    {
        AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
// #if DEBUG
//         Debugger.Launch();
//         Harmony.DEBUG = true;
//         Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", $@"{AssemblyPath}\harmony.log.txt");
// #endif

        try
        {
            Preferences = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            Preferences = new Preferences();
        }
        Preferences.PropertyChanged += SaveSettingsOnFile;

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
        if (IsEnabledRecording) {
            StateManager.StartRecording();
        }
    }

    [OnLevelEnd]
    public static void OnLevelEnd() {
        StateManager.Terminate();
        if (IsEnabledRecording) {
            StateManager.EndRecording();
        }
    }

    #region Menu Items
    [MainMenuItemSetting]
    public static ToggleRecording ToggleRecording(object factory, GuiFormat format)
    {
        return new ToggleRecording();
    }
    #endregion

    private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        try
        {
            XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Preferences);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
        }
    }
}
