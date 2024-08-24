using System.Collections.Generic;
using BetterVanilla.Components;
using BetterVanilla.Core.Helpers;

namespace BetterVanilla.Core;

public sealed class FeaturesManager
{
    private readonly HashSet<string> _availableHashes = [];
    private readonly HashSet<string> _hashedCodes = [];

    public FeaturesManager()
    {
        foreach (var featureCode in BetterVanillaManager.Instance.Database.Data.FeatureCodes)
        {
            _hashedCodes.Add(StringUtils.CalculateSHA256(featureCode));
        }
    }

    public void RegisterHash(string hash)
    {
        _availableHashes.Add(hash);
    }

    public void RegisterCode(string featureCode)
    {
        var hash = StringUtils.CalculateSHA256(featureCode);
        Ls.LogInfo($"Trying to register code: {featureCode} => [{hash}]");
        if (!_availableHashes.Contains(hash)) return;
        if (!BetterVanillaManager.Instance.Database.Data.FeatureCodes.Add(featureCode))
        {
            Ls.LogInfo($"Removing hash: {hash}");
            BetterVanillaManager.Instance.Database.Data.FeatureCodes.Remove(featureCode);
            _hashedCodes.Remove(hash);
        }
        else
        {
            Ls.LogInfo($"Added hash: {hash}");
            _hashedCodes.Add(hash);
        }
        BetterVanillaManager.Instance.Database.Save();
    }

    public bool IsUnlocked(string hash) => _hashedCodes.Contains(hash);

    public bool IsLocked(string hash) => !IsUnlocked(hash);
}