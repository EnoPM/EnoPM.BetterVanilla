using System;
using EnoPM.BetterVanilla.Components;
using Hazel;

namespace EnoPM.BetterVanilla.Core.Settings;

public sealed class BoolSetting : CustomSetting
{
    private bool _value;

    public ToggleSettingItem ToggleSettingBehaviour;

    public event Action<bool> ValueChanged;

    public static implicit operator bool(BoolSetting boolSetting)
    {
        return boolSetting._value;
    }

    public BoolSetting(string id, string title, bool defaultValue = default, SaveTypes saveType = SaveTypes.Local, Func<bool> isEditableFunc = null) : base(id, title, saveType, isEditableFunc)
    {
        _value = ResolveValue(defaultValue);
    }

    public void SetValue(bool value)
    {
        _value = value;
        ToggleSettingBehaviour?.SetValue(_value);
    }

    public override void CreateSettingUi(SettingsTabController settingsTabController)
    {
        ToggleSettingBehaviour = settingsTabController.CreateToggleOption();
        ToggleSettingBehaviour.SetSetting(this);
        ToggleSettingBehaviour.SetTitle(Title);
        ToggleSettingBehaviour.SetValue(_value);
        
        ToggleSettingBehaviour.AddOnValueChangedListener(OnSettingBehaviourValueChanged);
    }
    
    public override void Save() => Save(_value);

    protected override void OnSettingBehaviourValueChanged()
    {
        _value = ToggleSettingBehaviour.GetSettingValue();
        base.OnSettingBehaviourValueChanged();
        ValueChanged?.Invoke(_value);
    }

    public override SettingItem GetSettingBehaviour()
    {
        return ToggleSettingBehaviour;
    }

    public override void UiUpdate()
    {
        base.UiUpdate();
        var isEditable = IsEditable();
        ToggleSettingBehaviour.toggle.interactable = isEditable;
    }

    protected override void SetValueFromMessageReader(MessageReader reader)
    {
        SetValue(reader.ReadBoolean());
    }
    protected override void WriteValueInMessageWriter(MessageWriter writer)
    {
        writer.Write(_value);
    }
}