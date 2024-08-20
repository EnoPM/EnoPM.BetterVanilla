using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;

namespace EnoPM.BetterVanilla.Buttons;

public sealed class ModMenuHudButton : CustomGameplayButton
{
    public static ModMenuHudButton Instance { get; private set; }
    
    private void Start()
    {
        LabelText.SetText("Menu");
        MaxCooldown = 0f;
        SetUsesRemainingActive(false);
        SetLabelActive(true);
        var color = Utils.ColorFromHex("#2A70C8FF");
        SetLabelOutlineColor(color);
        SetLabelFaceColor(color);
        SetSprite(Utils.LoadSpriteFromResource("EnoPM.BetterVanilla.Resources.ModMenuButton.png"));

        Instance = this;
    }

    public override void OnClicked()
    {
        base.OnClicked();
        ModMenuController.Instance.Open();
    }

    protected override bool IsVisible()
    {
        return base.IsVisible() && true;
    }

    protected override bool CanClick()
    {
        return base.CanClick() && ModMenuController.Instance;
    }
}