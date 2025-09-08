using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class FeatureCodeBehaviour : MonoBehaviour
{
    public static FeatureCodeBehaviour? Instance { get; private set; }
    
    public Dictionary<string, List<string>> Codes { get; set; } = new();
    public Dictionary<string, List<string>> CosmeticCodes { get; set; } = new();
    public HashSet<string> SponsorCosmetics { get; set; } = [];
    public List<string> SponsorFriendCodes { get; set; } = [];
    private HashSet<string> AvailableHashes { get; } = [];
    private HashSet<string> LocalCodes { get; } = [];
    private HashSet<string> LocalHashes { get; } = [];

    private string GithubRepository { get; set; } = null!;
    private string GithubBranch { get; set; } = null!;
    private string GithubFilePath { get; set; } = null!;
    private string? PrivateKey { get; set; }
    private Coroutine? RefreshFeatureRegistryCoroutine { get; set; }
    
    public void ReloadFeatureRegistry()
    {
        if (RefreshFeatureRegistryCoroutine != null) return;
        RefreshFeatureRegistryCoroutine = this.StartCoroutine(CoReloadFeatureRegistry());
    }

    public void RegisterHash(string hash)
    {
        AvailableHashes.Add(hash);
    }

    public FeatureCodeResult ApplyCode(string code)
    {
        var hash = StringUtils.CalculateSHA256(code);
        Ls.LogMessage($"Applying code: '{code}'. Corresponding hash: '{hash}'");
        if (!AvailableHashes.Contains(hash))
        {
            return FeatureCodeResult.Invalid;
        }
        if (!CanBeUnlocked(hash) && !CosmeticCodes.ContainsKey(hash))
        {
            return FeatureCodeResult.Unauthorized;
        }
        if (!TryAddLocalCode(code))
        {
            LocalCodes.Remove(code);
            LocalHashes.Remove(hash);
            Ls.LogInfo($"Code '{code}' successfully disabled!");
            Save();
            return FeatureCodeResult.Disabled;
        }
        LocalHashes.Add(hash);
        Ls.LogInfo($"Code '{code}' successfully enabled!");
        Save();
        return FeatureCodeResult.Enabled;
    }

    public bool IsCosmeticUnlockable(string productId)
    {
        return CosmeticCodes.Any(x => x.Value.Contains(productId));
    }

    public bool IsCosmeticUnlocked(string productId)
    {
        var hash = CosmeticCodes.FirstOrDefault(x => x.Value.Contains(productId)).Key;
        return hash != null && LocalHashes.Contains(hash);
    }

    public bool IsUnlocked(string hash)
    {
        return LocalHashes.Contains(hash) && CanBeUnlocked(hash);
    }

    private bool TryAddLocalCode(string code)
    {
        var added = LocalCodes.Add(code);
        if (added)
        {
            Save();
        }
        return added;
    }

    private void Save()
    {
        using var file = File.Create(ModPaths.FeatureCodeFile);
        using var writer = new BinaryWriter(file);
        
        writer.Write(LocalCodes.Count);

        foreach (var code in LocalCodes)
        {
            writer.Write(code);
        }
    }

    private void Load()
    {
        LocalCodes.Clear();
        if (!File.Exists(ModPaths.FeatureCodeFile)) return;
        using var file = File.OpenRead(ModPaths.FeatureCodeFile);
        using var reader = new BinaryReader(file);
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var code = reader.ReadString();
            var hash = StringUtils.CalculateSHA256(code);
            LocalCodes.Add(code);
            LocalHashes.Add(hash);
        }
    }

    private void Awake()
    {
        PrivateKey = Environment.GetEnvironmentVariable("BETTERVANILLA_PRIVATE_KEY");
        
        GithubRepository = BepInExUpdater.GithubRepository;
        GithubFilePath = "features-registry.json";
        GithubBranch = "master";
        
        Instance = this;
    }

    private void Start()
    {
        Load();
        ReloadFeatureRegistry();
    }

    private bool CanBeUnlocked(string hash)
    {
        if (!EOSManager.InstanceExists || string.IsNullOrEmpty(EOSManager.Instance.FriendCode))
        {
            return false;
        }
        if (!Codes.TryGetValue(hash, out var friendCodes))
        {
            return false;
        }
        return friendCodes.Contains(EOSManager.Instance.FriendCode);
    }

    private IEnumerator CoReloadFeatureRegistry()
    {
        var requestTask = RequestUtils.GetAsync<FeaturesRegistry>($"https://raw.githubusercontent.com/{GithubRepository}/refs/heads/{GithubBranch}/{GithubFilePath}");
        while (!requestTask.IsCompleted)
        {
            if (requestTask.Exception != null)
            {
                Ls.LogWarning(requestTask.Exception.Message);
                RefreshFeatureRegistryCoroutine = null;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (requestTask.Result != null)
        {
            SponsorFriendCodes = requestTask.Result.ContributorFriendCodes;
            Codes = requestTask.Result.FeatureHashPermissions;
            CosmeticCodes = requestTask.Result.HashedCosmetics;
            SponsorCosmetics = requestTask.Result.SponsorCosmetics.ToHashSet();

            foreach (var cosmeticHash in CosmeticCodes.Keys)
            {
                RegisterHash(cosmeticHash);
            }
        }
        
        RefreshFeatureRegistryCoroutine = null;
    }
}