using HarmonyLib;
using Rewired;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(InputManager_Base))]
internal static class InputManagerBasePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(InputManager_Base.Update))]
    private static bool UpdatePrefix()
    {
        return !UiInteractionBlocker.ShouldBlock();
    }
}