using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.BetterVanilla;

[BepInProcess("Among Us"), BepInPlugin(PluginProps.Guid, PluginProps.Name, PluginProps.Version)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin : BasePlugin
{
    internal static Plugin Instance { get; private set; }
    internal static ManualLogSource Logger { get; private set; }
    internal static ConfigFile ConfigFile { get; private set; }
    private static readonly Harmony HarmonyPatcher = new(PluginProps.Guid);
    
    private static GameObject UiObject { get; set; }
    
    public override void Load()
    {
        Instance = this;
        
        Logger = Log;
        ConfigFile = Config;
        
        ModConfigs.Load();
        HarmonyPatcher.PatchAll();
        CustomButtonsManager.RegisterAssembly(Assembly.GetExecutingAssembly());

        UiObject = new GameObject("BetterVanillaUi")
        {
            layer = 4,
            hideFlags = HideFlags.HideAndDontSave
        };
        
        AddComponent<StartupBehaviour>();
        Log.LogInfo($"Plugin {PluginProps.Guid} is loaded!");
    }
}