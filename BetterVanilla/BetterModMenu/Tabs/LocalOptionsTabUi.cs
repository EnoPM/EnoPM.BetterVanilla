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

    protected override void Update()
    {
        base.Update();
        foreach (var option in AllOptions)
        {
            option.RefreshVisibility();
        }
    }
}