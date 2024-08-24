using System;
using System.Collections;
using AmongUs.Data;
using BetterVanilla.Components;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class ProgressionScreenExtensions
{
    public static IEnumerator CoDoAnimations(this ProgressionScreen progressionScreen, ProgressionManager.XpGrantResult xpGainedResult)
    {
        yield return Effects.Wait(0.2f);
        if (((int)xpGainedResult.MaxLevel != (int)DataManager.Player.Stats.Level || (int)xpGainedResult.OldLevel != (int)xpGainedResult.NewLevel) && xpGainedResult.GrantedXp > 0U)
        {
            Ls.LogInfo($"Running vanilla AnimateXpAndLevelUp");
            yield return progressionScreen.AnimateXpAndLevelUp(xpGainedResult);
        }
        else
        {
            var xpManager = BetterVanillaManager.Instance.Xp;
            xpManager.SetupCache(xpGainedResult);
            progressionScreen.InitModdedProgressionScreen();
            if (xpManager.MaxLevel != DataManager.Player.Stats.Level + BetterVanillaManager.Instance.Database.Data.PlayerLevel && xpManager.GrantedXp > 0)
            {
                yield return progressionScreen.CoAnimateXpAndLevelUp();
            }
        }
        yield return progressionScreen.AnimatePodsAndBeans();
    }
    
    private static void InitModdedProgressionScreen(this ProgressionScreen progressionScreen)
    {
        var xpManager = BetterVanillaManager.Instance.Xp;
        var oldLevel = DataManager.Player.Stats.Level + xpManager.OldLevel;
        var newLevel = DataManager.Player.Stats.Level + xpManager.NewLevel;
        progressionScreen.XpBar.Value = progressionScreen.XpBar.CapValue = xpManager.OldXpAmount;
        progressionScreen.XpBar.MaxValue = xpManager.XpRequiredToLevelUp;
        progressionScreen.XpBar.GlowAlpha = 0.0f;
        progressionScreen.XpEarnedNowText.text = TranslationController.Instance.GetString(StringNames.XpGainedValue, xpManager.GrantedXp);
        progressionScreen.XpEarnedNowText.color = Palette.ClearWhite;
        progressionScreen.FutureLevelText.color = Palette.White;
        if (oldLevel == xpManager.MaxLevel)
        {
            progressionScreen.XpBar.Value = 1f;
            progressionScreen.XpBar.CapValue = 1f;
            progressionScreen.XpBar.MaxValue = 1f;
            progressionScreen.LevelText.text = TranslationController.Instance.GetString(StringNames.Max);
            progressionScreen.FutureLevelText.color = Palette.ClearWhite;
        }
        else if (newLevel == xpManager.MaxLevel)
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
    
    private static IEnumerator CoAnimateXpAndLevelUp(this ProgressionScreen progressionScreen)
    {
        var xpManager = BetterVanillaManager.Instance.Xp;
        var maxLevel = xpManager.MaxLevel;
        var currentPlayerLevel = DataManager.Player.Stats.Level + BetterVanillaManager.Instance.Database.Data.PlayerLevel;
        
        var xpRequiredToLevelUp = xpManager.XpRequiredToLevelUp;
        var oldXpAmount = xpManager.OldXpAmount;
        var grantedXp = xpManager.GrantedXp;
        var levelledUp = xpManager.LevelledUp;
        var xpRequiredToLevelUpNextLevel = xpManager.XpRequiredToLevelUpNextLevel;
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
        xpManager.ApplyAndClearCache();
    }
}