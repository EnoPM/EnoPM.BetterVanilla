using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Components;
using BetterVanilla.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu;

public sealed class BetterModMenuButtonUi : MonoBehaviour
{
    public Button button = null!;
    public ZoomUi zoom = null!;
    public AutoTaskButtonUi autoTaskButton = null!;

    public void OnButtonClicked()
    {
        if (BetterVanillaManager.Instance == null) return;
        BetterVanillaManager.Instance.Menu.Show();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        zoom.gameObject.SetActive(LocalConditions.CanZoom());
        autoTaskButton.gameObject.SetActive(LocalConditions.CanCompleteAutoTasks());
    }
}