using BepInEx.Configuration;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

internal static class ModConfigs
{
    private const string ExperimentalHash = "EEA8F808064B3607D09D1C86260365C1C6125EEBD44E5D673C0545E767822657";
    
    private static readonly ConfigEntry<string> ExperimentalCodeEntry;
    private static readonly ConfigEntry<KeyCode> ZoomInKeyEntry;
    private static readonly ConfigEntry<KeyCode> ZoomOutKeyEntry;
    private static readonly ConfigEntry<string> CheaterColorEntry;
    private static readonly ConfigEntry<string> HostColorEntry;
    private static readonly ConfigEntry<string> ImpostorColorEntry;
    private static readonly ConfigEntry<string> NoTasksDoneColorEntry;
    private static readonly ConfigEntry<string> LessThanHalfTasksDoneColorEntry;
    private static readonly ConfigEntry<string> MoreThanHalfTasksDoneColorEntry;
    private static readonly ConfigEntry<string> AllTasksDoneColorEntry;
    
    internal static bool IsExperimental { get; set; }
    internal static bool CheckAmDead { get; set; }
    internal static bool CheckAmImpostor { get; set; }
    internal static KeyCode ZoomInKey { get; set; }
    internal static KeyCode ZoomOutKey { get; set; }
    internal static Color CheaterColor { get; set; }
    internal static Color HostColor { get; set; }
    internal static Color ImpostorColor { get; set; }
    internal static Color NoTasksDoneColor { get; set; }
    internal static Color LessThanHalfTasksDoneColor { get; set; }
    internal static Color MoreThanHalfTasksDoneColor { get; set; }
    internal static Color AllTasksDoneColor { get; set; }

    static ModConfigs()
    {
        var configFile = Plugin.ConfigFile;
        ExperimentalCodeEntry = configFile.Bind("Debug", "Experimental", string.Empty, "License key to enable experimental features");

        ZoomInKeyEntry = configFile.Bind("Zoom", "Zoom in key", KeyCode.KeypadPlus, "Keyboard key code to zoom in");
        ZoomOutKeyEntry = configFile.Bind("Zoom", "Zoom out key", KeyCode.KeypadMinus, "Keyboard key code to zoom out");

        CheaterColorEntry = configFile.Bind("Colors", "Cheater", Utils.ColorToHex(Palette.ImpostorRed), "Color to show when a cheater is detected");
        HostColorEntry = configFile.Bind("Colors", "Host", Utils.ColorToHex(Color.magenta), "Color to show host");
        ImpostorColorEntry = configFile.Bind("Colors", "Impostor", Utils.ColorToHex(Palette.ImpostorRed), "Color to show impostors");
        NoTasksDoneColorEntry = configFile.Bind("Colors", "No tasks done", Utils.ColorToHex(new Color(0.76f, 0.16f, 0.52f, 1f)), "Color to show when no tasks are done");
        LessThanHalfTasksDoneColorEntry = configFile.Bind("Colors", "Less than half tasks done", Utils.ColorToHex(new Color(0.79f, 0.40f, 0.63f, 1f)), "Color to show when less than half tasks are done");
        MoreThanHalfTasksDoneColorEntry = configFile.Bind("Colors", "More than half tasks done", Utils.ColorToHex(new Color(0.9f, 0.76f, 0.78f, 1f)), "Color to show when more than half tasks are done");
        AllTasksDoneColorEntry = configFile.Bind("Colors", "All tasks done", Utils.ColorToHex(new Color(0.34f, 1f, 0.69f, 1f)), "Color to show when all tasks are done");
    }

    internal static void Load()
    {
        IsExperimental = ExperimentalCodeEntry.Value != string.Empty && Utils.CalculateSHA256(ExperimentalCodeEntry.Value) == ExperimentalHash;
        CheckAmDead = true;
        CheckAmImpostor = true;
        
        ZoomInKey = ZoomInKeyEntry.Value;
        ZoomOutKey = ZoomOutKeyEntry.Value;
        
        CheaterColor = Utils.ColorFromHex(CheaterColorEntry.Value);
        HostColor = Utils.ColorFromHex(HostColorEntry.Value);
        ImpostorColor = Utils.ColorFromHex(ImpostorColorEntry.Value);
        NoTasksDoneColor = Utils.ColorFromHex(NoTasksDoneColorEntry.Value);
        LessThanHalfTasksDoneColor = Utils.ColorFromHex(LessThanHalfTasksDoneColorEntry.Value);
        MoreThanHalfTasksDoneColor = Utils.ColorFromHex(MoreThanHalfTasksDoneColorEntry.Value);
        AllTasksDoneColor = Utils.ColorFromHex(AllTasksDoneColorEntry.Value);
    }
}