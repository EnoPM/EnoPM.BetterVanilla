using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(NormalGameManager))]
internal static class NormalGameManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(NormalGameManager.InitComponents))]
    private static bool InitComponentsPrefix(NormalGameManager __instance)
    {
        Plugin.Logger.LogMessage($"NormalGameManager.InitComponents Prefix");
        __instance.LogicFlow = __instance.AddGameLogic(new LogicGameFlowNormal(__instance));
        __instance.LogicMinigame = __instance.AddGameLogic(new LogicMinigame(__instance));
        __instance.LogicRoleSelection = __instance.AddGameLogic(new LogicRoleSelectionNormal(__instance));
        __instance.LogicUsables = __instance.AddGameLogic(new LogicUsablesBasic(__instance));
        __instance.LogicOptions = __instance.AddGameLogic(new LogicOptionsNormal(__instance));
        return false;
    }
}