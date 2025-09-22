using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Manager;

public abstract class BaseCosmeticManager<TCosmetic, TViewData, TParent, TCosmeticData>
    where TCosmetic : BaseCosmetic<TViewData, TParent, TCosmeticData>
    where TViewData : ScriptableObject
    where TParent : MonoBehaviour
    where TCosmeticData : CosmeticData
{

    protected List<TCosmetic> UnregisteredCosmetics { get; } = [];
    protected Dictionary<string, TCosmetic> RegisteredCosmetics { get; } = new();
    internal Dictionary<string, TViewData> ViewDataCache { get; } = new();
    protected HashSet<TParent> ParentCache { get; } = [];

    protected abstract bool CanBeCached(TParent parent);
    
    protected abstract void PopulateParent(TParent parent);
    protected abstract List<TCosmeticData> GetVanillaCosmeticData();
    protected abstract void OverrideVanillaCosmeticData(List<TCosmeticData> allCosmeticData);
    protected abstract TParent? GetPlayerParent(PlayerControl player);

    public virtual void RegisterCosmetics()
    {
        if(UnregisteredCosmetics.Count == 0) return;
        var vanillaCosmetics = GetVanillaCosmeticData();
        var noneCosmetic = vanillaCosmetics.First();
        vanillaCosmetics.Remove(noneCosmetic);
        var customCosmetics = new List<TCosmeticData>
        {
            noneCosmetic
        };
        var cache = UnregisteredCosmetics.ToList();
        cache.Reverse();
        foreach (var cosmetic in cache)
        {
            customCosmetics.Add(cosmetic.ToCosmeticData());
            UnregisteredCosmetics.Remove(cosmetic);
            RegisteredCosmetics.Add(cosmetic.ProductId, cosmetic);
        }
        
        customCosmetics.AddRange(vanillaCosmetics);
        OverrideVanillaCosmeticData(customCosmetics);
    }

    public virtual void AddCosmetic(TCosmetic cosmetic)
    {
        if (IsAlreadyExisting(cosmetic))
        {
            Ls.LogError($"[{GetType().Name}] Unable to add cosmetic {cosmetic.ProductId}: a cosmetic with the same name already exists");
            return;
        }
        UnregisteredCosmetics.Add(cosmetic);
    }

    public virtual void RefreshEquippedCosmetics(TCosmetic cosmetic)
    {
        var parents = ParentCache.Where(cosmetic.IsMyParent);
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
        if (!TryGetViewData(cosmetic.ProductId, out var viewData)) return;
        cosmetic.ToViewData(viewData);
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

    public bool IsCustomCosmetic(string key) => ViewDataCache.ContainsKey(key);

    public bool TryGetViewDataByName(string name, [MaybeNullWhen(false)] out TViewData viewData)
    {
        foreach (var item in ViewDataCache.Values)
        {
            if (item.name == name)
            {
                viewData = item;
                return true;
            }
        }
        viewData = null;
        return false;
    }

    public bool TryGetCosmetic(string key, [MaybeNullWhen(false)] out TCosmetic cosmetic)
    {
        return RegisteredCosmetics.TryGetValue(key, out cosmetic);
    }

    public List<TCosmetic> GetAllRegisteredCosmetics()
    {
        return RegisteredCosmetics.Values.ToList();
    }

    public void UpdateAnimationFrames()
    {
        foreach (var cosmetic in GetAllRegisteredCosmetics())
        {
            cosmetic.AnimateFrames();
        }
    }

    public abstract void RefreshAnimationFrames(PlayerPhysics playerPhysics);

    public abstract void UpdateMaterialFromViewAsset(TParent parent, TViewData asset);
    
    public abstract void PopulateParentFromAsset(TParent parent, TViewData asset);
    
    public bool IsAlreadyExisting(TCosmetic cosmetic) => UnregisteredCosmetics.Any(x => x.ProductId == cosmetic.ProductId) || RegisteredCosmetics.Values.Any(x => x.ProductId == cosmetic.ProductId);
}