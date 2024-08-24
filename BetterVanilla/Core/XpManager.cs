using System;
using BetterVanilla.Components;

namespace BetterVanilla.Core;

public sealed class XpManager
{
    private readonly float _baseXpPerLevel;
    private readonly float _exponent;
    public readonly uint MaxLevel;
    
    public uint OldLevel { get; private set; }
    public uint OldXpAmount { get; private set; }
    public uint GrantedXp { get; private set; }
    public uint NewXp { get; private set; }
    public uint NewLevel { get; private set; }
    public uint XpRequiredToLevelUp { get; private set; }
    public uint XpRequiredToLevelUpNextLevel { get; private set; }
    public bool LevelledUp { get; private set; }

    public XpManager()
    {
        _baseXpPerLevel = 100f;
        _exponent = 0.56f;
        MaxLevel = 200;
    }
    
    public uint CalculateLevel(uint xp)
    {
        var level = (float)Math.Pow(xp / _baseXpPerLevel, _exponent);
        var roundedLevel = (uint)Math.Floor(level);
        return Math.Min(roundedLevel, MaxLevel);
    }
    
    private uint CalculateXpForLevel(uint level)
    {
        if (level > MaxLevel)
        {
            return 0;
        }
        
        var xp = _baseXpPerLevel * Math.Pow(level, 1.0 / _exponent);
        return (uint)xp;
    }

    public void SetupCache(ProgressionManager.XpGrantResult xpGrantResult)
    {
        OldXpAmount = BetterVanillaManager.Instance.Database.Data.PlayerExp;
        OldLevel = CalculateLevel(OldXpAmount);
        GrantedXp = xpGrantResult.GrantedXp;
        NewXp = OldXpAmount + GrantedXp;
        NewLevel = OldLevel == MaxLevel ? OldLevel : OldLevel + 1;
        XpRequiredToLevelUp = CalculateXpForLevel(NewLevel);
        XpRequiredToLevelUpNextLevel = CalculateXpForLevel(NewLevel + 1);
        LevelledUp = OldXpAmount + GrantedXp >= XpRequiredToLevelUp;
    }

    public void ApplyAndClearCache()
    {
        Ls.LogMessage($"{nameof(XpManager)} {nameof(ApplyAndClearCache)}");
        var db = BetterVanillaManager.Instance.Database;
        db.Data.PlayerExp = NewXp;
        db.Data.PlayerLevel = CalculateLevel(NewXp);
        db.Save();
        
        OldLevel = 0;
        OldXpAmount = 0;
        GrantedXp = 0;
        NewXp = 0;
        NewLevel = 0;
        XpRequiredToLevelUp = 0;
        XpRequiredToLevelUpNextLevel = 0;
        LevelledUp = false;
    }
}