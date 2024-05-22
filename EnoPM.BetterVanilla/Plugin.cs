using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.ManagedComponents;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace EnoPM.BetterVanilla;

[BepInProcess("Among Us"), BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin : BasePlugin
{
    internal static ManualLogSource Logger { get; private set; }
    internal static ConfigFile ConfigFile { get; private set; }
    private static readonly Harmony HarmonyPatcher = new(MyPluginInfo.PLUGIN_GUID);
    
    private static GameObject UiObject { get; set; }
    public static PopupController SuccessPopup { get; private set; }
    public static PopupController ErrorPopup { get; private set; }
    public static UpdaterController Updater { get; private set; }
    
    public override void Load()
    {
        Logger = Log;
        ConfigFile = Config;
        
        ModConfigs.Load();
        
        RegisterManagedComponentsInIl2Cpp();

        UiObject = new GameObject("BetterVanillaUI")
        {
            layer = 4,
            hideFlags = HideFlags.HideAndDontSave
        };

        SuccessPopup = ModAssets.SuccessPopupPrefab.Instantiate(UiObject.transform);
        ErrorPopup = ModAssets.ErrorPopupPrefab.Instantiate(UiObject.transform);
        Updater = ModAssets.UpdaterPrefab.Instantiate(UiObject.transform);
        
        AddComponent<ModStampBehaviour>();
        HarmonyPatcher.PatchAll();
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static void RegisterManagedComponentsInIl2Cpp()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var ns = $"{typeof(Plugin).Namespace}.{nameof(ManagedComponents)}";
        var managedComponentsTypes = types.Where(x => x.Namespace == ns && x.IsAssignableTo(typeof(MonoBehaviour)));
        foreach (var type in managedComponentsTypes)
        {
            if (ClassInjector.IsTypeRegisteredInIl2Cpp(type)) continue;
            ClassInjector.RegisterTypeInIl2Cpp(type);
        }
    }
}