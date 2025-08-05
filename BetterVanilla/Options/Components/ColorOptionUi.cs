using System;
using BetterVanilla.Core.Helpers;
using UnityEngine.UI;
using TMPro;
using BetterVanilla.Options.Components.Controllers;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;
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

    private static Color[] PredefinedColors { get; } =
    [
        ColorUtils.FromHex("#FF4C4C"),
        ColorUtils.FromHex("#FFA500"),
        ColorUtils.FromHex("#FFD700"),
        ColorUtils.FromHex("#32CD32"),

        ColorUtils.FromHex("#42A5F5"),
        ColorUtils.FromHex("#B57EDC"),
        ColorUtils.FromHex("#A0522D"),
        ColorUtils.FromHex("#F5F5F5"),

        ColorUtils.FromHex("#FF69B4"),
        ColorUtils.FromHex("#00FFFF"),
        ColorUtils.FromHex("#7CFC00"),
        ColorUtils.FromHex("#FF7F50"),

        ColorUtils.FromHex("#FF77FF"),
        ColorUtils.FromHex("#6A5ACD"),
        ColorUtils.FromHex("#E6E6FA"),
        ColorUtils.FromHex("#20B2AA")
    ];

    private ColorLocalOption? SerializableOption { get; set; }

    public void SetOption(ColorLocalOption option)
    {
        SerializableOption = option;
        SerializableOption.SetUiOption(this);
        SerializableOption.RefreshUiOption();
    }

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
        if (SerializableOption == null) return;
        SerializableOption.Value = color;
    }

    private void SetRgbColor(Color color)
    {
        red.SetValueWithoutNotify(color.r * 255f);
        green.SetValueWithoutNotify(color.g * 255f);
        blue.SetValueWithoutNotify(color.b * 255f);
    }

    private Color GetRgbColor() => new(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 255f);

    private void RefreshHexFieldAndPreview()
    {
        var color = GetRgbColor();
        SetPreviewColor(color);
        hexField.SetTextWithoutNotify(ColorUtils.ToHex(color, false));
    }

    private void Update()
    {
        SerializableOption?.RefreshUiLock();
    }

    public override void RefreshVisibility()
    {
        SerializableOption?.RefreshUiVisibility();
    }
}