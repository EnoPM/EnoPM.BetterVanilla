using UnityEngine;

namespace BetterVanilla.BetterModMenu.Core;

public abstract class BaseMenuTabUi : MonoBehaviour
{
    public TabHeaderUi tabHeader = null!;
    public GameObject tabBody = null!;
    public Transform tabContent = null!;
    
    public bool IsSelected => tabBody.activeSelf;

    public void Hide() => SetSelected(false);
    
    public void Show() => SetSelected(true);

    private void SetSelected(bool value)
    {
        tabBody.SetActive(value);
        tabHeader.SetEnabled(!value);
        enabled = value;
    }
}