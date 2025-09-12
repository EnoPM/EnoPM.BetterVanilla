using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Core;
using BetterVanilla.Options;

namespace BetterVanilla.BetterModMenu.Tabs;

public sealed class LocalOptionsTabUi : BaseOptionsTabUi
{
    private void Start()
    {
        InitSerializableOptions(LocalOptions.Default);
        InitSerializableOptions(FeatureOptions.Default);
        ApplyOptionFilters();
    }
}