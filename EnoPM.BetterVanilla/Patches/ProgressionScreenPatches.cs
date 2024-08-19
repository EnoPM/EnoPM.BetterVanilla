using System;
using System.Collections;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Core;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(ProgressionScreen))]
internal static class ProgressionScreenPatches
{
    /*
    [HarmonyPrefix, HarmonyPatch(nameof(ProgressionScreen.DoAnimations))]
    private static bool DoAnimationsPrefix(ProgressionScreen __instance, ProgressionManager.XpGrantResult xpGainedResult)
    {
        __instance.StartCoroutine(CoDoAnimations(__instance, xpGainedResult));
        return false;
    }
    */
    
    [HarmonyPrefix, HarmonyPatch(nameof(ProgressionScreen.AnimateXpAndLevelUp))]
    private static bool AnimateXpAndLevelUpPrefix(ProgressionScreen __instance, ProgressionManager.XpGrantResult xpGainedResult)
    {
        Plugin.Logger.LogMessage($"AnimateXpAndLevelUpPrefix called!");
        return true;
    }

    private static Coroutine DoAnimationsCoroutine { get; set; }

    [HarmonyPrefix, HarmonyPatch(typeof(ProgressionScreen._DoAnimations_d__14), nameof(ProgressionScreen._DoAnimations_d__14.MoveNext))]
    private static bool _DoAnimations_d__14MoveNextPrefix(ProgressionScreen._DoAnimations_d__14 __instance)
    {
        if (DoAnimationsCoroutine != null && !DoAnimationsCoroutine.WasCollected)
        {
            return false;
        }
        var progressionScreen = __instance.__4__this;
        DoAnimationsCoroutine = progressionScreen.StartCoroutine(CoDoAnimations(progressionScreen, __instance.xpGainedResult));
        return false;
    }

    private static IEnumerator CoDoAnimations(ProgressionScreen progressionScreen, ProgressionManager.XpGrantResult xpGainedResult)
    {
        yield return Effects.Wait(0.2f);
        if (((int)xpGainedResult.MaxLevel != (int)DB.Player.PlayerLevel || (int)xpGainedResult.OldLevel != (int)xpGainedResult.NewLevel) && xpGainedResult.GrantedXp > 0U)
        {
            yield return CoAnimateXpAndLevelUp(progressionScreen, xpGainedResult);
        }
        yield return progressionScreen.AnimatePodsAndBeans();
        DoAnimationsCoroutine = null;
    }

    private static IEnumerator CoAnimateXpAndLevelUp(ProgressionScreen progressionScreen, ProgressionManager.XpGrantResult xpGainedResult)
    {
        xpGainedResult = XpManager.GetModdedXpGrantResult(xpGainedResult);
        XpManager.UpdateLocalLevel(xpGainedResult);
        Plugin.Logger.LogMessage($"Xp needed: {xpGainedResult.XpRequiredToLevelUp} {xpGainedResult.OldXpAmount}");
        progressionScreen.LevelText.SetText($"{DB.Player.PlayerLevel}");
        Plugin.Logger.LogMessage($"{nameof(CoAnimateXpAndLevelUp)}: {xpGainedResult.MaxLevel} {DB.Player.PlayerLevel}");
        var newXpAmount = (ulong)Math.Min(xpGainedResult.XpRequiredToLevelUp, xpGainedResult.OldXpAmount + xpGainedResult.GrantedXp);
        var oldXpAmount = xpGainedResult.OldXpAmount;
        progressionScreen.XpBar.CapValue = newXpAmount;
        progressionScreen.StartCoroutine(Effects.Lerp(1.25f, new Action<float>(progressionScreen._AnimateXpAndLevelUp_b__15_0)));
        progressionScreen.StartCoroutine(Effects.ColorFade(progressionScreen.XpEarnedNowText, Palette.ClearWhite, Color.white, 0.625f));
        progressionScreen.StartCoroutine(progressionScreen.AnimateXpBarFill(oldXpAmount, newXpAmount));
        yield return Effects.Wait(0.625f);
        yield return Effects.ColorFade(progressionScreen.XpEarnedNowText, Color.white, Palette.ClearWhite, 0.625f);
        if (xpGainedResult.LevelledUp)
        {
            if (DB.Player.PlayerLevel >= xpGainedResult.MaxLevel)
            {
                progressionScreen.LevelText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MaxLevel);
                progressionScreen.FutureLevelText.color = Palette.ClearWhite;
            }
            else
            {
                progressionScreen.LevelText.text = $"{DB.Player.PlayerLevel}";
                progressionScreen.XpBar.MaxValue = xpGainedResult.XpRequiredToLevelUpNextLevel;
            }
            newXpAmount = xpGainedResult.GrantedXp + xpGainedResult.OldXpAmount - xpGainedResult.XpRequiredToLevelUp;
            progressionScreen.XpBar.Value = 0.0f;
            progressionScreen.XpBar.GlowAlpha = 1f;
            progressionScreen.XpBar.CapValue = xpGainedResult.XpRequiredToLevelUpNextLevel;
            progressionScreen.XpBar.MaxValue = xpGainedResult.XpRequiredToLevelUpNextLevel;
            progressionScreen.XpEarnedNowText.gameObject.SetActive(false);
            SoundManager.Instance.PlaySound(progressionScreen.LevelUpSound, false, 0.5f);
            yield return Effects.ScaleIn(progressionScreen.LevelCircle, 2f, 1f, 0.5f);
            yield return Effects.All(progressionScreen.XpBar.ScaleDownY(0.75f), Effects.Lerp(0.75f, new Action<float>(progressionScreen._AnimateXpAndLevelUp_b__15_1)));
            yield return Effects.Wait(0.2f);
            var playerLevel = DB.Player.PlayerLevel;
            if (playerLevel >= xpGainedResult.MaxLevel)
            {
                progressionScreen.FutureLevelText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Max);
            }
            else
            {
                progressionScreen.FutureLevelText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.LevelShorthand, $"{playerLevel}");
            }

            progressionScreen.StartCoroutine(Effects.Lerp(0.75f, new Action<float>(progressionScreen._AnimateXpAndLevelUp_b__15_2)));
            yield return progressionScreen.AnimateXpBarFill(0U, newXpAmount);
        }
    }
}