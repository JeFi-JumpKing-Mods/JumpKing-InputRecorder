using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

// using InputRecorder.Menu;

using JumpKing.Mods;
using JumpKing.PauseMenu;
using JumpKing;
using JumpKing.MiscSystems.Achievements;
using InputRecorder.States;
using JumpKing.GameManager;

namespace InputRecorder;

[JumpKingMod(IDENTIFIER)]
public static class InputRecorder
{
    const string IDENTIFIER = "JeFi.InputRecorder";
    const string HARMONY_IDENTIFIER = "JeFi.InputRecorder.Harmony";
    const string SETTINGS_FILE = "JeFi.InputRecorder.Preferences.xml";
    const string BINDING_FILE = "TasBinding.xml";
    const string TAS_FOLDER = "TAS";

    public static string AssemblyPath { get; set; }
    // public static Preferences Preferences { get; private set; }
    public static TasBinding TasBinding { get; private set; }
    public static string MapName = string.Empty;
    public static PlayerStats CurrentStats;

    [BeforeLevelLoad]
    public static void BeforeLevelLoad()
    {
        AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
        Debugger.Launch();
        Harmony.DEBUG = true;
        Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", $@"{AssemblyPath}\harmony.log.txt");
#endif
//         try
//         {
//             Preferences = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
//         }
//         catch (Exception e)
//         {
//             Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
//             Preferences = new Preferences();
//         }
//         Preferences.PropertyChanged += SaveSettingsOnFile;

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

        MapName = GetMapName();
        Type achievementManagerType = AccessTools.TypeByName("JumpKing.MiscSystems.Achievements.AchievementManager");
        object achievementManagerinstance = AccessTools.Field(achievementManagerType, "instance").GetValue(null);
        MethodInfo GetCurrentStats = AccessTools.Method(achievementManagerType, "GetCurrentStats");
        CurrentStats = (PlayerStats)GetCurrentStats.Invoke(achievementManagerinstance, null);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = $"{timestamp}_{Sanitize(MapName)}_A{CurrentStats.attempts}-S{CurrentStats.session}.tas";
        string filePath = Path.Combine(AssemblyPath, TAS_FOLDER, fileName);

        StateManager.StartRecording(filePath, GameLoop.m_player);
    }

    [OnLevelEnd]
    public static void OnLevelEnd() {
        StateManager.EndRecording();

        MapName = string.Empty;
        CurrentStats = new();
    }

    // #region Menu Items
    // [PauseMenuItemSetting]
    // [MainMenuItemSetting]
    // public static ToggleInputRecorder ToggleInputRecorder(object factory, GuiFormat format)
    // {
    //     return new ToggleInputRecorder();
    // }
    // #endregion

    // private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
    // {
    //     try
    //     {
    //         XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Preferences);
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
    //     }
    // }

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
