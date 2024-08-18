using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class TabHeaderController : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI titleText;
    public Button button;

    private Sprite _sprite;
    private string _titleText;

    private void Awake()
    {
        if (_sprite)
        {
            icon.sprite = _sprite;
        }
        if (!string.IsNullOrEmpty(_titleText))
        {
            titleText.SetText(_titleText);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
        if (icon)
        {
            icon.sprite = _sprite;
        }
    }

    public void SetTitleText(string title)
    {
        _titleText = title;
        if (titleText)
        {
            titleText.SetText(_titleText);
        }
    }
}