using System;
using AmongUs.Data;

namespace EnoPM.BetterVanilla.Core;

public static class XpManager
{
    private const float XpPerLevelBase = 100f;
    private const float Exponent = 0.56f;
    public const uint MaxLevel = 200;

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
        Plugin.Logger.LogMessage($"{nameof(UpdateLocalLevel)}: {xpGrantResult.GrantedXp > 0} {DB.Player.PlayerLevel} {CalculateLevel(DB.Player.PlayerExp + xpGrantResult.GrantedXp)}");
        if (xpGrantResult.GrantedXp == 0) return;
        DB.Player.PlayerExp += xpGrantResult.GrantedXp;
        DB.Player.PlayerLevel = CalculateLevel(DB.Player.PlayerExp);
        DB.SavePlayer();
    }
    
    public static uint OldLevel { get; private set; }
    public static uint OldXpAmount { get; private set; }
    public static uint GrantedXp { get; private set; }
    public static uint NewXp { get; private set; }
    public static uint NewLevel { get; private set; }
    public static uint XpRequiredToLevelUp { get; private set; }
    public static uint XpRequiredToLevelUpNextLevel { get; private set; }
    public static bool LevelledUp { get; private set; }

    public static void SetupCache(ProgressionManager.XpGrantResult xpGrantResult)
    {
        OldLevel = DB.Player.PlayerLevel;
        OldXpAmount = DB.Player.PlayerExp;
        GrantedXp = xpGrantResult.GrantedXp;
        NewXp = OldXpAmount + GrantedXp;
        NewLevel = OldLevel == MaxLevel ? OldLevel : OldLevel + 1;
        XpRequiredToLevelUp = CalculateXpForLevel(NewLevel);
        XpRequiredToLevelUpNextLevel = CalculateXpForLevel(NewLevel + 1);
        LevelledUp = OldXpAmount + GrantedXp >= XpRequiredToLevelUp;
    }

    public static void ApplyAndClearCache()
    {
        DB.Player.PlayerExp = NewXp;
        DB.Player.PlayerLevel = CalculateLevel(NewXp);
        DB.SavePlayer();
        ClearCache();
    }

    private static void ClearCache()
    {
        OldLevel = 0;
        OldXpAmount = 0;
        GrantedXp = 0;
        NewXp = 0;
        NewLevel = 0;
        XpRequiredToLevelUp = 0;
        XpRequiredToLevelUpNextLevel = 0;
        LevelledUp = false;
    }

    public static void Log(this ProgressionManager.XpGrantResult xp)
    {
        Plugin.Logger.LogMessage($"GrantedXp: {xp.GrantedXp}");
        Plugin.Logger.LogMessage($"OldXpAmount: {xp.OldXpAmount}");
        Plugin.Logger.LogMessage($"XpRequiredToLevelUp: {xp.XpRequiredToLevelUp}");
        Plugin.Logger.LogMessage($"XpRequiredToLevelUpNextLevel: {xp.XpRequiredToLevelUpNextLevel}");
        Plugin.Logger.LogMessage($"LevelledUp: {xp.LevelledUp}");
        Plugin.Logger.LogMessage($"OldLevel: {xp.OldLevel}");
        Plugin.Logger.LogMessage($"NewLevel: {xp.NewLevel}");
        Plugin.Logger.LogMessage($"MaxLevel: {xp.MaxLevel}");
    }
}