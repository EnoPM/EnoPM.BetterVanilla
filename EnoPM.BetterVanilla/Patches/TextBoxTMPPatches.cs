using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(TextBoxTMP))]
public static class TextBoxTMPPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(TextBoxTMP.IsCharAllowed))]
    private static void IsCharAllowedPrefix(TextBoxTMP __instance, char i, out bool __result)
    {
        if (__instance.allowAllCharacters && i.GetHashCode() != 524296)
        {
            __result = true;
            return;
        }
        __result = __instance.IpMode ? i is >= '0' and <= '9' or '.' : i == ' ' || i is >= 'A' and <= 'Z' || i is >= 'a' and <= 'z' || i is >= '0' and <= '9' || i is >= 'À' and <= 'ÿ' || i is >= 'Ѐ' and <= 'џ' || i is >= '\u3040' and <= '㆟' || i is >= 'ⱡ' and <= '힣' || __instance.AllowSymbols && TextBoxTMP.SymbolChars.Contains(i) || __instance.AllowEmail && TextBoxTMP.EmailChars.Contains(i);
    }
}