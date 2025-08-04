using BetterVanilla.BetterModMenu.Tabs;
using BetterVanilla.Components;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.GeneratedRuntime;
using BetterVanilla.Options.Components;
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
        localOptionsTab.enabled = false;
    }

    private void Start()
    {
        OpenSponsorTab();
        Hide();
    }

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

    public override void Hide()
    {
        base.Hide();
        BetterVanillaManager.Instance.BetterMenu.ButtonUi.Show();
    }
}