using JetBrains.Annotations;

namespace BetterVanilla.Cosmetics.Api.Resources;

[UsedImplicitly]
public sealed class AssetBundleFile : Resource
{
    public List<AssetBundleEntry> Entries { get; set; } = [];
}