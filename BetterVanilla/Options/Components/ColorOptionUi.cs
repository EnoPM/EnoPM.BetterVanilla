using System;
using BetterVanilla.Core.Helpers;
using UnityEngine.UI;
using TMPro;
using BetterVanilla.Options.Components.Controllers;
using UnityEngine;

namespace BetterVanilla.Options.Components;

public sealed class ColorOptionUi : BaseOptionUi
{
    public ColorShadeUi red = null!;
    public ColorShadeUi green = null!;
    public ColorShadeUi blue = null!;

    public TMP_InputField hexField = null!;
    public Image preview = null!;

    public Transform chipContainer = null!;
    public ColorChipUi chipPrefab = null!;

    private static Color[] PredefinedColors { get; } = [
        //ColorUtils.FromHex("#C61111"),
        ColorUtils.FromHex("#132ED2"),
        ColorUtils.FromHex("#11802D"),
        ColorUtils.FromHex("#EE54BB"),
        ColorUtils.FromHex("#F07D0D"),
        ColorUtils.FromHex("#F6F657"),
        ColorUtils.FromHex("#3F474E"),
        //ColorUtils.FromHex("#D7E1F1"),
        ColorUtils.FromHex("#6B2FBC"),
        ColorUtils.FromHex("#71491E"),
        ColorUtils.FromHex("#38FFDD"),
        ColorUtils.FromHex("#50F039"),
        ColorUtils.FromHex("#5F1D2E"),
        ColorUtils.FromHex("#ECC0D3"),
        ColorUtils.FromHex("#F0E7A8"),
        ColorUtils.FromHex("#758593"),
        ColorUtils.FromHex("#918877"),
        ColorUtils.FromHex("#D76464")
    ];

    public void SetColor(Color color)
    {
        hexField.SetTextWithoutNotify(ColorUtils.ToHex(color, false));
        
        SetPreviewColor(color);
        SetRgbColor(color);
    }
    
    private void Awake()
    {
        red.ShadeUpdated += RefreshHexFieldAndPreview;
        green.ShadeUpdated += RefreshHexFieldAndPreview;
        blue.ShadeUpdated += RefreshHexFieldAndPreview;
        
        hexField.onEndEdit
            .AddListener(new Action<string>(OnHexValueChanged));
    }
    
    private void Start()
    {
        foreach (var color in PredefinedColors)
        {
            var chip = Instantiate(chipPrefab, chipContainer);
            chip.SetColor(color);
            chip.SetClickHandler(SetColor);
        }
    }

    private void OnHexValueChanged(string value)
    {
        if (!ColorUtils.IsValidHexColor(value))
        {
            hexField.SetTextWithoutNotify(ColorUtils.ToHex(GetRgbColor(), false));
            return;
        }
        var color = ColorUtils.FromHex(value);
        SetPreviewColor(color);
        SetRgbColor(color);
    }
    
    private void SetPreviewColor(Color color)
    {
        preview.color = color;
    }

    private void SetRgbColor(Color color)
    {
        red.SetValueWithoutNotify(color.r * 255f);
        green.SetValueWithoutNotify(color.g * 255f);
        blue.SetValueWithoutNotify(color.b * 255f);
    }

    private Color GetRgbColor() => new (red.Value / 255f, green.Value / 255f, blue.Value / 255f, 255f);

    private void RefreshHexFieldAndPreview()
    {
        var color = GetRgbColor();
        preview.color = color;
        hexField.SetTextWithoutNotify(ColorUtils.ToHex(color, false));
    }
}