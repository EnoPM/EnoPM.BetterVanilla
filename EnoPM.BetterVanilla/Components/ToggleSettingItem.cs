using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class ToggleSettingItem : SettingItem
{
    public Toggle toggle;

    public void SetValue(bool value)
    {
        toggle.Set(value);
    }
}