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
    private static Coroutine DoAnimationsCoroutine { get; set; }

    [HarmonyPrefix, HarmonyPatch(typeof(ProgressionScreen._DoAnimations_d__14), nameof(ProgressionScreen._DoAnimations_d__14.MoveNext))]
    private static bool _DoAnimations_d__14MoveNextPrefix(ProgressionScreen._DoAnimations_d__14 __instance)
    {
        var progressionScreen = __instance.__4__this;
        progressionScreen.StartCoroutine(CoDoAnimations(progressionScreen, __instance.xpGainedResult));
        return false;
    }

    private static void InitModdedProgressionScreen(ProgressionScreen progressionScreen)
    {
        var oldLevel = DataManager.Player.Stats.Level + XpManager.OldLevel;
        var newLevel = DataManager.Player.Stats.Level + XpManager.NewLevel;
        progressionScreen.XpBar.Value = progressionScreen.XpBar.CapValue = XpManager.OldXpAmount;
        progressionScreen.XpBar.MaxValue = XpManager.XpRequiredToLevelUp;
        progressionScreen.XpBar.GlowAlpha = 0.0f;
        progressionScreen.XpEarnedNowText.text = TranslationController.Instance.GetString(StringNames.XpGainedValue, XpManager.GrantedXp);
        progressionScreen.XpEarnedNowText.color = Palette.ClearWhite;
        progressionScreen.FutureLevelText.color = Palette.White;
        if (oldLevel == XpManager.MaxLevel)
        {
            progressionScreen.XpBar.Value = 1f;
            progressionScreen.XpBar.CapValue = 1f;
            progressionScreen.XpBar.MaxValue = 1f;
            progressionScreen.LevelText.text = TranslationController.Instance.GetString(StringNames.Max);
            progressionScreen.FutureLevelText.color = Palette.ClearWhite;
        }
        else if (newLevel == XpManager.MaxLevel)
        {
            progressionScreen.FutureLevelText.text = TranslationController.Instance.GetString(StringNames.Max);
            progressionScreen.LevelText.text = ProgressionManager.FormatVisualLevel(oldLevel);
        }
        else
        {
            progressionScreen.LevelText.text = ProgressionManager.FormatVisualLevel(oldLevel);
            progressionScreen.FutureLevelText.text = TranslationController.Instance.GetString(StringNames.LevelShorthand, ProgressionManager.FormatVisualLevel(newLevel == oldLevel ? oldLevel + 1U : newLevel));
        }
    }

    private static IEnumerator CoDoAnimations(ProgressionScreen progressionScreen, ProgressionManager.XpGrantResult xpGainedResult)
    {
        yield return Effects.Wait(0.2f);
        if (((int)xpGainedResult.MaxLevel != (int)DataManager.Player.Stats.Level || (int)xpGainedResult.OldLevel != (int)xpGainedResult.NewLevel) && xpGainedResult.GrantedXp > 0U)
        {
            Plugin.Logger.LogInfo($"Running vanilla AnimateXpAndLevelUp");
            yield return progressionScreen.AnimateXpAndLevelUp(xpGainedResult);
        }
        else
        {
            XpManager.SetupCache(xpGainedResult);
            InitModdedProgressionScreen(progressionScreen);
            if (XpManager.MaxLevel != DataManager.Player.Stats.Level + DB.Player.PlayerLevel && XpManager.GrantedXp > 0)
            {
                yield return AnimateXpAndLevelUp(progressionScreen);
            }
        }
        yield return progressionScreen.AnimatePodsAndBeans();
        DoAnimationsCoroutine = null;
    }

    private static IEnumerator AnimateXpAndLevelUp(ProgressionScreen progressionScreen)
    {
        const uint maxLevel = XpManager.MaxLevel;
        var currentPlayerLevel = DataManager.Player.Stats.Level + DB.Player.PlayerLevel;
        
        var xpRequiredToLevelUp = XpManager.XpRequiredToLevelUp;
        var oldXpAmount = XpManager.OldXpAmount;
        var grantedXp = XpManager.GrantedXp;
        var levelledUp = XpManager.LevelledUp;
        var xpRequiredToLevelUpNextLevel = XpManager.XpRequiredToLevelUpNextLevel;
        var nextPlayerLevel = levelledUp ? currentPlayerLevel + 1 : currentPlayerLevel;
        
        var newXpAmount = (ulong)Math.Min(xpRequiredToLevelUp, oldXpAmount + grantedXp);
        progressionScreen.XpBar.CapValue = newXpAmount;
        progressionScreen.StartCoroutine(Effects.Lerp(1.25f, (Action<float>)(t =>
        {
            progressionScreen.XpEarnedNowText.transform.SetLocalX(0.9f + progressionScreen.XpEarnedNowCurve.Evaluate(t));
        })));
        progressionScreen.StartCoroutine(Effects.ColorFade(progressionScreen.XpEarnedNowText, Palette.ClearWhite, Color.white, 0.625f));
        progressionScreen.StartCoroutine(progressionScreen.AnimateXpBarFill(oldXpAmount, newXpAmount));
        yield return Effects.Wait(0.625f);
        yield return Effects.ColorFade(progressionScreen.XpEarnedNowText, Color.white, Palette.ClearWhite, 0.625f);
        if (levelledUp)
        {
            if (currentPlayerLevel >= maxLevel)
            {
                progressionScreen.LevelText.SetText(TranslationController.Instance.GetString(StringNames.MaxLevel));
                progressionScreen.FutureLevelText.color = Palette.ClearWhite;
            }
            else
            {
                progressionScreen.LevelText.SetText(ProgressionManager.FormatVisualLevel(nextPlayerLevel));
                progressionScreen.XpBar.MaxValue = xpRequiredToLevelUpNextLevel;
            }
            newXpAmount = grantedXp + oldXpAmount - xpRequiredToLevelUp;
            progressionScreen.XpBar.Value = 0f;
            progressionScreen.XpBar.GlowAlpha = 1f;
            progressionScreen.XpBar.CapValue = xpRequiredToLevelUpNextLevel;
            progressionScreen.XpBar.MaxValue = xpRequiredToLevelUpNextLevel;
            progressionScreen.XpEarnedNowText.gameObject.SetActive(false);
            SoundManager.Instance.PlaySound(progressionScreen.LevelUpSound, false, 0.5f);
            yield return Effects.ScaleIn(progressionScreen.LevelCircle, 2f, 1f, 0.5f);
            yield return Effects.All([
                progressionScreen.XpBar.ScaleDownY(0.75f),
                Effects.Lerp(0.75f, new Action<float>(t =>
                {
                    var transform = progressionScreen.FutureLevelText.transform;
                    var localPosition = transform.localPosition;
                    transform.localPosition = new Vector3(localPosition.x, Mathf.Lerp(0f, -0.3f, t), localPosition.z);
                    progressionScreen.FutureLevelText.color = progressionScreen.FutureLevelText.color.SetAlpha(1f - t);
                }))
            ]);
            yield return Effects.Wait(0.2f);
            var num = nextPlayerLevel + 1;
            if (num >= maxLevel)
            {
                progressionScreen.FutureLevelText.SetText(TranslationController.Instance.GetString(StringNames.Max));
            }
            else
            {
                progressionScreen.FutureLevelText.SetText(TranslationController.Instance.GetString(StringNames.LevelShorthand, ProgressionManager.FormatVisualLevel(num)));
            }
            progressionScreen.StartCoroutine(Effects.Lerp(0.75f, (Action<float>)(t =>
            {
                var transform = progressionScreen.FutureLevelText.transform;
                var localPosition = transform.localPosition;
                transform.localPosition = new Vector3(localPosition.x, Mathf.Lerp(0.3f, 0f, t), localPosition.z);
                progressionScreen.FutureLevelText.color = progressionScreen.FutureLevelText.color.SetAlpha(t);
            })));
            yield return progressionScreen.AnimateXpBarFill(0U, newXpAmount);
        }
        XpManager.ApplyAndClearCache();
    }
}