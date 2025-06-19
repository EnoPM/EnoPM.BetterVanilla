using System;
using BetterVanilla.BetterModMenu.Tabs;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.GeneratedRuntime;
using TMPro;

namespace BetterVanilla.BetterModMenu;

public sealed class BetterModMenuUi : BaseWindowUi
{
    public TextMeshProUGUI versionText = null!;

    public SponsorTabUi sponsorTab = null!;
    public LocalOptionsTabUi localOptionsTab = null!;

    private void Awake()
    {
        versionText.SetText($"v{GeneratedProps.Version}");
    }

    private void Start() => OpenSponsorTab();

    public void OpenSponsorTab()
    {
        localOptionsTab.Hide();
        sponsorTab.Show();
    }

    public void OpenLocalOptionsTab()
    {
        sponsorTab.Hide();
        localOptionsTab.Show();
    }
}