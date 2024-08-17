using TMPro;

namespace EnoPM.BetterVanilla.Components;

public class DropdownSettingItem : SettingItem
{
    public TMP_Dropdown dropdown;

    public void SetValue(int index)
    {
        dropdown.SetValue(index);
    }
}