using BetterVanilla.BetterModMenu.Tabs;
using BetterVanilla.Components;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.Core.Extensions;
using BetterVanilla.GeneratedRuntime;
using TMPro;

namespace BetterVanilla.BetterModMenu;

public sealed class BetterModMenuUi : BaseWindowUi
{
    public TextMeshProUGUI versionText = null!;

    public HomeTabUi homeTab = null!;
    public SponsorTabUi sponsorTab = null!;
    public LocalOptionsTabUi localOptionsTab = null!;

    private void Awake()
    {
        versionText.SetText($"v{GeneratedProps.Version}");
        sponsorTab.enabled = false;
        localOptionsTab.enabled = false;
    }

    private void Start()
    {
        OpenHomeTab();
        Hide();
    }

    public void OpenHomeTab()
    {
        sponsorTab.Hide();
        localOptionsTab.Hide();
        homeTab.Show();
    }

    public void OpenSponsorTab()
    {
        homeTab.Hide();
        localOptionsTab.Hide();
        sponsorTab.Show();
    }

    public void OpenLocalOptionsTab()
    {
        homeTab.Hide();
        sponsorTab.Hide();
        localOptionsTab.Show();
    }

    private void OnEnable()
    {
        PlayerControl.LocalPlayer.SetMovement(false);
    }
    
    private void OnDisable()
    {
        PlayerControl.LocalPlayer.SetMovement(true);
    }

    public override void Hide()
    {
        base.Hide();
        BetterVanillaManager.Instance.BetterMenu.ButtonUi.Show();
    }
}