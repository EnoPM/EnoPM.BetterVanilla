using HarmonyLib;
using TMPro;
using UnityEngine;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(GameSettingMenu))]
internal static class GameSettingMenuPatches
{
    private static TextMeshPro MenuDescriptionText { get; set; }
    
    [HarmonyPostfix, HarmonyPatch(nameof(GameSettingMenu.Start))]
    private static void StartPostfix(GameSettingMenu __instance)
    {
        __instance.GamePresetsButton.gameObject.SetActive(false);
        __instance.PresetsTab.gameObject.SetActive(false);

        __instance.GameSettingsButton.gameObject.SetActive(false);
        __instance.GameSettingsTab.gameObject.SetActive(false);
        
        __instance.MenuDescriptionText.gameObject.SetActive(false);
        
        if (!MenuDescriptionText)
        {
            MenuDescriptionText = Object.Instantiate(__instance.MenuDescriptionText, __instance.MenuDescriptionText.transform.parent);
            MenuDescriptionText.name = $"BetterVanilla{nameof(MenuDescriptionText)}";
            MenuDescriptionText.GetComponent<TextTranslatorTMP>().TargetText = StringNames.RoleSettingsDescription;
            MenuDescriptionText.gameObject.SetActive(true);
        }
        
        Plugin.Logger.LogMessage($"{nameof(MenuDescriptionText)}: {MenuDescriptionText.name} {MenuDescriptionText.enabled} {MenuDescriptionText.text}");
    }

    [HarmonyPrefix, HarmonyPatch(nameof(GameSettingMenu.ChangeTab), typeof(int), typeof(bool))]
    private static void ChangeTabPrefix(GameSettingMenu __instance, ref int tabNum, bool previewOnly)
    {
        tabNum = 2;
    }
}