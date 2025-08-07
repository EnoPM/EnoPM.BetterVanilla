using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Core;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.BetterModMenu.Tabs;

public sealed class SponsorTabUi : BaseOptionsTabUi
{
    public BecomeSponsorUi becomeSponsor = null!;
    
    private void Start()
    {
        InitSerializableOptions(SponsorOptions.Default);
        ApplyOptionFilters();
    }

    protected override void Update()
    {
        base.Update();
        /*
        if (Input.anyKeyDown)
        {
            var key = Input.inputString;
            Ls.LogMessage($"Key pressed: {key}");
        }
        */
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Ls.LogMessage($"F2 pressed");
        }
    }
}