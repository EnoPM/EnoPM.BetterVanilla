using System;
using System.Collections.Generic;
using System.Linq;
using Cpp2IL.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Cosmetics.Patches;

[HarmonyPatch(typeof(HatManager))]
internal static class HatManagerPatches
{
    private static bool _hatIsRunning;
    private static bool _hatIsLoaded;
    private static List<HatData>? _allHats;
        
    [HarmonyPrefix, HarmonyPatch(nameof(HatManager.GetHatById))]
    private static void GetHatByIdPrefix(HatManager __instance)
    {
        if (_hatIsRunning || _hatIsLoaded) return;
        _hatIsRunning = true;
        // Maybe we can use lock keyword to ensure simultaneous list manipulations ?
        _allHats = __instance.allHats.ToList();
        var cache = CosmeticsManager.UnregisteredHats.Clone();
        foreach (var hat in cache)
        {
            try
            {
                _allHats.Add(CosmeticsManager.CreateHatBehaviour(hat));
                CosmeticsManager.UnregisteredHats.Remove(hat);
                CosmeticsManager.RegisteredHats[hat.Name] = hat;
            }
            catch (Exception err)
            {
                CosmeticsPlugin.Logging.LogWarning($"GetHatByIdPrefix: error for hat {hat.Name}: {err}");
            }
        }
        cache.Clear();

        __instance.allHats = _allHats.ToArray();
        _hatIsLoaded = true;
    }
        
    [HarmonyPostfix, HarmonyPatch(nameof(HatManager.GetHatById))]
    private static void GetHatByIdPostfix()
    {
        _hatIsRunning = false;
    }
}