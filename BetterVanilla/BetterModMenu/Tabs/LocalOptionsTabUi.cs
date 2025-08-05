using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Core;
using BetterVanilla.Options;

namespace BetterVanilla.BetterModMenu.Tabs;

public sealed class LocalOptionsTabUi : BaseOptionsTabUi
{
    private void Start()
    {
        Ls.LogMessage($"Started");
        InitSerializableOptions(LocalOptions.Default);
        ApplyOptionFilters();
    }

    private void Update()
    {
        foreach (var option in AllOptions)
        {
            option.RefreshVisibility();
        }
    }
}