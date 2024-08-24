using System;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.Core.Options;
using TMPro;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu.Settings;

public sealed class StringSettingBehaviour : BaseSettingBehaviour
{
    public TextMeshProUGUI valueText;
    public Button increaseButton;
    public Button decreaseButton;

    private StringLocalOption StringOption { get; set; }

    public int Index { get; private set; }
    public IntRange ValidRange { get; set; }

    public override void Initialize(BaseLocalOption option)
    {
        if (option is not StringLocalOption stringOption)
        {
            throw new Exception($"{nameof(BaseLocalOption)} must be {nameof(StringLocalOption)}");
        }
        StringOption = stringOption;
        base.Initialize(option);
    }

    private void Awake()
    {
        increaseButton.onClick.AddListener(new Action(OnIncreaseButtonClick));
        decreaseButton.onClick.AddListener(new Action(OnDecreaseButtonClick));
    }

    protected override void Start()
    {
        ValidRange = new IntRange(0, StringOption.Values.Count - 1);
        base.Start();
    }

    private void OnIncreaseButtonClick()
    {
        if (!ValidRange.Contains(Index + 1)) return;
        Index++;
        UpdateOptionValue();
        UpdateValueText();
        AdjustButtonStates();
    }

    private void OnDecreaseButtonClick()
    {
        if (!ValidRange.Contains(Index - 1)) return;
        Index--;
        UpdateOptionValue();
        UpdateValueText();
        AdjustButtonStates();
    }

    private void UpdateOptionValue()
    {
        StringOption.Index = Index;
    }

    private void UpdateValueText()
    {
        valueText.SetText(StringOption.Values[Index]);
    }

    private void AdjustButtonStates()
    {
        increaseButton.interactable = ValidRange.Contains(Index + 1);
        decreaseButton.interactable = ValidRange.Contains(Index - 1);
    }

    public override void UpdateFromOption()
    {
        Index = StringOption.Index;
        UpdateValueText();
        AdjustButtonStates();
    }
}