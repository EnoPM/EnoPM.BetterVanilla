using System;
using AmongUs.Data;

namespace EnoPM.BetterVanilla.Core;

public static class XpManager
{
    private const float XpPerLevelBase = 100f;
    private const float Exponent = 0.6f;
    private const int MaxLevel = 200;

    public static uint CalculateLevel(uint xp)
    {
        var level = (float)Math.Pow(xp / XpPerLevelBase, Exponent);
        var roundedLevel = (uint)Math.Floor(level);
        return Math.Min(roundedLevel, MaxLevel);
    }
    
    private static uint CalculateXpForLevel(uint level)
    {
        if (level > MaxLevel)
        {
            return 0;
        }
        
        var xp = XpPerLevelBase * Math.Pow(level, 1.0 / Exponent);
        return (uint)xp;
    }

    public static void UpdateLocalLevel(ProgressionManager.XpGrantResult xpGrantResult)
    {
        if (xpGrantResult.GrantedXp <= 0) return;
        DB.Player.PlayerExp += xpGrantResult.GrantedXp;
        DB.Player.PlayerLevel = CalculateLevel(DB.Player.PlayerExp);
        DB.SavePlayer();
    }

    public static ProgressionManager.XpGrantResult GetModdedXpGrantResult(ProgressionManager.XpGrantResult oldXpResult)
    {
        var oldLevel = DB.Player.PlayerLevel;
        var oldXpAmount = DB.Player.PlayerExp;
        var grantedXp = oldXpResult.GrantedXp;
        var newXp = oldXpAmount + grantedXp;
        var newLevel = CalculateLevel(newXp);
        var xpRequiredToLevelUp = CalculateXpForLevel(newLevel + 1);
        var xpRequiredToLevelUpNextLevel = CalculateXpForLevel(newLevel + 2);
        var levelledUp = newLevel > oldLevel;
        
        return new ProgressionManager.XpGrantResult(
            grantedXp,
            oldXpAmount,
            xpRequiredToLevelUp,
            xpRequiredToLevelUpNextLevel,
            levelledUp,
            oldLevel,
            newLevel,
            MaxLevel
        );
    }

    public static EndGameResult GetModdedEndGameResult(EndGameResult baseEndGameResult)
    {
        return new EndGameResult(
            baseEndGameResult.GameOverReason,
            baseEndGameResult.ShowAd,
            GetModdedXpGrantResult(baseEndGameResult.XpGrantResult),
            baseEndGameResult.PodsGrantResult,
            baseEndGameResult.BeansGrantResult
        );
    }
}