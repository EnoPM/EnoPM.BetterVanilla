using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BetterVanilla.Cosmetics.Api.Core;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Manager;

public abstract class BaseCosmeticManager<TCosmetic, TViewData, TParent, TCosmeticData>
    where TCosmetic : ICosmeticItem
    where TViewData : ScriptableObject
    where TParent : MonoBehaviour
    where TCosmeticData : CosmeticData
{
    public Material? CachedMaterial { get; private set; }

    protected List<TCosmetic> UnregisteredCosmetics { get; } = [];
    protected Dictionary<string, TCosmetic> RegisteredCosmetics { get; } = new();
    protected Dictionary<string, TViewData> ViewDataCache { get; } = new();
    protected HashSet<TParent> ParentCache { get; } = [];

    protected abstract bool CanBeCached(TParent parent);
    
    protected abstract void HydrateViewData(TViewData viewData, TCosmetic cosmetic);
    protected abstract void HydrateCosmeticData(TCosmeticData cosmeticData, TCosmetic cosmetic);
    protected abstract string GetCosmeticProductId(TCosmetic cosmetic);
    protected abstract bool IsParentCosmetic(TParent parent, TCosmetic cosmetic);
    protected abstract void PopulateParent(TParent parent);
    protected abstract List<TCosmeticData> GetVanillaCosmeticData();
    protected abstract void OverrideVanillaCosmeticData(List<TCosmeticData> allCosmeticData);
    protected abstract TParent? GetPlayerParent(PlayerControl player);

    public virtual void RegisterCosmetics()
    {
        if(UnregisteredCosmetics.Count == 0) return;
        CosmeticsPlugin.Logging.LogMessage($"Registering {UnregisteredCosmetics.Count} cosmetics");
        var vanillaCosmetics = GetVanillaCosmeticData();
        var noneCosmetic = vanillaCosmetics.First();
        vanillaCosmetics.Remove(noneCosmetic);
        var customCosmetics = new List<TCosmeticData>
        {
            noneCosmetic
        };
        var cache = UnregisteredCosmetics.ToList();
        foreach (var cosmetic in cache)
        {
            customCosmetics.Add(CreateCosmeticData(cosmetic));
            UnregisteredCosmetics.Remove(cosmetic);
            RegisteredCosmetics.Add(cosmetic.Name, cosmetic);
        }
        
        customCosmetics.AddRange(vanillaCosmetics);
        OverrideVanillaCosmeticData(customCosmetics);
    }

    public abstract void AddCosmetic(TCosmetic cosmetic);
    
    public virtual TCosmeticData CreateCosmeticData(TCosmetic cosmetic)
    {
        CosmeticsPlugin.Logging.LogMessage($"Creating cosmetic data for {cosmetic.Name}");
        if (CachedMaterial == null)
        {
            CachedMaterial = HatManager.Instance.PlayerMaterial;
        }

        var viewData = CreateViewData(cosmetic.Name);
        HydrateViewData(viewData, cosmetic);
        
        var cosmeticData = CreateCosmeticData(cosmetic.Name);
        HydrateCosmeticData(cosmeticData, cosmetic);
        
        return cosmeticData;
    }

    public virtual void RefreshEquippedCosmetics(TCosmetic cosmetic)
    {
        var parents = ParentCache
            .Where(x => IsParentCosmetic(x, cosmetic));
        foreach (var parent in parents)
        {
            PopulateParent(parent);
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var parent = GetPlayerParent(player);
            if (parent == null) continue;
            PopulateParent(parent);
        }
            
    }
    
    public virtual void OnCosmeticLoaded(TCosmetic cosmetic)
    {
        if (!TryGetViewData(cosmetic.Name, out var viewData)) return;
        HydrateViewData(viewData, cosmetic);
        RefreshEquippedCosmetics(cosmetic);
    }

    public void CacheParent(TParent parent)
    {
        if (!CanBeCached(parent)) return;
        ParentCache.Add(parent);
    }
    
    public bool TryGetViewData(string key, [MaybeNullWhen(false)] out TViewData viewData)
    {
        return ViewDataCache.TryGetValue(key, out viewData);
    }

    public bool TryGetCosmetic(string key, [MaybeNullWhen(false)] out TCosmetic cosmetic)
    {
        return RegisteredCosmetics.TryGetValue(key, out cosmetic);
    }

    public List<TCosmetic> GetAllRegisteredCosmetics()
    {
        return RegisteredCosmetics.Values.ToList();
    }
    
    public abstract void UpdateAnimationFrames();

    protected TViewData CreateViewData(string name)
    {
        var viewData = ScriptableObject.CreateInstance<TViewData>();
        ViewDataCache.Add(name, viewData);
        return viewData;
    }

    protected TCosmeticData CreateCosmeticData(string cosmeticName)
    {
        var cosmeticData = ScriptableObject.CreateInstance<TCosmeticData>();
        cosmeticData.name = cosmeticName;
        return cosmeticData;
    }
}