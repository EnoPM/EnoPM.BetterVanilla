using System;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Options.Components.Controllers;

public sealed class ColorChipUi : MonoBehaviour
{
    public Button button = null!;
    
    public Color Color { get; private set; }
    private Action<Color>? ClickHandler { get; set; }

    public void SetColor(Color color)
    {
        button.image.color = Color = color;
    }

    public void SetClickHandler(Action<Color>? handler)
    {
        ClickHandler = handler;
    }

    public void OnButtonClicked()
    {
        ClickHandler?.Invoke(Color);
    }
}