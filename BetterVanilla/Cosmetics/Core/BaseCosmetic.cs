using System.Collections.Generic;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core;

public abstract class BaseCosmetic<TViewData, TParent, TData> : ICosmeticItem
    where TViewData : ScriptableObject
    where TParent : MonoBehaviour
    where TData : CosmeticData

{
    public string Name { get; set; }

    public bool Adaptive { get; set; }
    
    public abstract string ProductId { get; }

    protected abstract Dictionary<string, TViewData> ViewDataCache { get; }

    public SerializedCosmeticAuthor? Author { get; set; }

    protected BaseCosmetic(string name, SerializedCosmeticAuthor? author)
    {
        Name = name;
        Author = author;
    }

    public abstract bool IsMyParent(TParent parent);
   
    public virtual void AnimateFrames()
    {
        
    }

    public virtual void RefreshAnimatedFrames(TParent parent, bool flipX)
    {
        
    }
    
    protected int UpdateAnimationFrame(List<Sprite>? frames, int currentFrame)
    {
        if (frames == null || frames.Count == 0) return 0;
        currentFrame++;
        if (currentFrame >= frames.Count)
        {
            currentFrame = 0;
        }
        return currentFrame;
    }

    public virtual TViewData ToViewData()
    {
        var viewData = ScriptableObject.CreateInstance<TViewData>();
        viewData.name = Name;
        ViewDataCache.Add(ProductId, viewData);
        return ToViewData(viewData);
    }

    public virtual TViewData ToViewData(TViewData viewData)
    {
        ViewDataCache.TryAdd(ProductId, viewData);
        return viewData;
    }

    public virtual TData ToCosmeticData()
    {
        var cosmeticData = ScriptableObject.CreateInstance<TData>();
        cosmeticData.name = Name;
        cosmeticData.Free = false;
        cosmeticData.BundleId = "BetterVanilla";
        return cosmeticData;
    }

    public string GetDisplayName()
    {
        return Author != null ? $"{Name}\n<size=50%>by {Author.Name}</size>" : Name;
    }
}