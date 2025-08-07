using TMPro;
using UnityEngine;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class ProgressBarUi : MonoBehaviour
{
    public RectTransform containerRect = null!;
    public RectTransform progressBarRect = null!;
    public TextMeshProUGUI progressText = null!;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetProgress(float progress)
    {
        if (containerRect == null || progressBarRect == null) return;
        progressBarRect.sizeDelta = new Vector2((containerRect.rect.width - 8f) * progress, progressBarRect.sizeDelta.y);
        if (progressText == null) return;
        progressText.SetText($"{Mathf.RoundToInt(progress * 100f)}%");
    }
}