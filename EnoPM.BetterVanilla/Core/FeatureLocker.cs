using System.Collections.Generic;
using EnoPM.BetterVanilla.Core.Settings;

namespace EnoPM.BetterVanilla.Core;

public static class FeatureLocker
{
    private static readonly HashSet<string> AvailableHashes = [];
    private static readonly HashSet<string> HashedCodes = [];

    private static bool IsUnlocked(string hashedCode) => HashedCodes.Contains(hashedCode);
    private static bool IsLocked(string hashedCode) => !IsUnlocked(hashedCode);

    private static void Load()
    {
        foreach (var code in DB.Player.FeatureCodes)
        {
            HashedCodes.Add(Utils.CalculateSHA256(code));
        }
    }

    public static void Reload()
    {
        HashedCodes.Clear();
        Load();
    }

    public static void RegisterCode(string code)
    {
        var hash = Utils.CalculateSHA256(code);
        Plugin.Logger.LogInfo($"Trying to register code: {code} [{hash}]");
        if (!AvailableHashes.Contains(hash)) return;
        HashedCodes.Add(hash);
        if (!DB.Player.FeatureCodes.Add(code))
        {
            DB.Player.FeatureCodes.Remove(code);
            HashedCodes.Remove(hash);
        }
        DB.SavePlayer();
    }

    public static void LockedByHash(this CustomSetting setting, string hash)
    {
        AvailableHashes.Add(hash);
        setting.SetIsLockedFunc(() => IsLocked(hash));
    }
}