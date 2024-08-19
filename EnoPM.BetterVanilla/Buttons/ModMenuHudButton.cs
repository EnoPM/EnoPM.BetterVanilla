using System;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Attributes;
using EnoPM.BetterVanilla.Core.Data;

namespace EnoPM.BetterVanilla.Buttons;

[ButtonConfig(
    position: ButtonPositions.BottomLeft
)]
public sealed class ModMenuHudButton : CustomGameplayButton
{
    private void Start()
    {
        LabelText.SetText("Better Vanilla");
        MaxCooldown = 0f;
        SetUsesRemainingActive(false);
        SetLabelActive(true);
        SetLabelOutlineColor(Utils.ColorFromHex("#2A70C8FF"));
        SetSprite(Utils.LoadSpriteFromResource("EnoPM.BetterVanilla.Resources.ModMenuButton.png"));
    }

    public override void OnClicked()
    {
        base.OnClicked();
        ModMenuController.Instance.Open();
    }

    protected override bool CanClick()
    {
        return base.CanClick() && ModMenuController.Instance;
    }
}