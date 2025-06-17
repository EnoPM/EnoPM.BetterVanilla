using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
using UnityEngine.Networking;

namespace BetterVanilla.Core;

public sealed class FeaturesManager
{
    private readonly HashSet<string> _availableHashes = [];
    private readonly HashSet<string> _hashedCodes = [];

    public FeaturesRegistry Registry { get; private set; } = null;

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

    public void Initialize(string githubRepository, string branchName, string filePath)
    {
        BetterVanillaManager.Instance.StartCoroutine(CoStart(githubRepository, branchName, filePath));
    }

    private IEnumerator CoStart(string githubRepository, string branchName, string filePath)
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl($"https://raw.githubusercontent.com/{githubRepository}/refs/heads/{branchName}/{filePath}");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            yield return null;
        }
        if (www.isNetworkError || www.isHttpError)
        {
            Ls.LogError(www.error);
            yield break;
        }
        Registry = JsonSerializer.Deserialize<FeaturesRegistry>(www.downloadHandler.text);
        www.downloadHandler.Dispose();
        www.Dispose();
        
        if (Registry == null)
        {
            Ls.LogError("No features registry found");
            yield break;
        }
        RefreshPlayerSponsorStates();
    }

    private static void RefreshPlayerSponsorStates()
    {
        foreach (var player in BetterVanillaManager.Instance.AllPlayers)
        {
            player.UpdateSponsorState();
        }
    }

    public void RegisterCode(string featureCode)
    {
        var hash = StringUtils.CalculateSHA256(featureCode);
        Ls.LogInfo($"Trying to register code: {featureCode} => [{hash}]");
        if (!_availableHashes.Contains(hash) || !IsUnlockable(hash)) return;
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

    private bool IsUnlockable(string hash)
    {
        var eos = EOSManager.Instance;
        if (Registry == null || !eos || string.IsNullOrEmpty(eos.FriendCode))
        {
            return false;
        }
        if (!Registry.FeatureHashPermissions.TryGetValue(hash, out var allowedFriendCodes))
        {
            return false;
        }
        if (!allowedFriendCodes.Contains(eos.FriendCode))
        {
            return false;
        }
        return true;
    }

    public bool IsUnlocked(string hash)
    {
        return _hashedCodes.Contains(hash) && IsUnlockable(hash);
    }

    public bool IsLocked(string hash) => !IsUnlocked(hash);
}