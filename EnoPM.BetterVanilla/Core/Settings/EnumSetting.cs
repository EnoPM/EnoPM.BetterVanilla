using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core.Attributes;
using Hazel;

namespace EnoPM.BetterVanilla.Core.Settings;

public sealed class EnumSetting<TEnum> : CustomSetting where TEnum : struct
{
    private TEnum _value;
    private readonly Dictionary<TEnum, string> _values = [];

    public DropdownSettingItem DropdownSettingBehaviour;
    public event Action<TEnum> ValueChanged;

    public static implicit operator TEnum(EnumSetting<TEnum> enumSetting)
    {
        return enumSetting._value;
    }

    public EnumSetting(string id, string title, TEnum defaultValue = default, SaveTypes saveType = SaveTypes.Local, Func<bool> isEditableFunc = null) : base(id, title, saveType, isEditableFunc)
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enum", enumType.FullName);
        }

        var enumValues = Enum.GetValues(enumType);

        foreach (var enumValue in enumValues)
        {
            var key = (TEnum)enumValue;
            var fieldName = key.ToString();
            if (fieldName == null)
            {
                throw new Exception($"Unable to stringify enum key {key} in {enumType.FullName}");
            }
            var field = enumType.GetField(fieldName);
            if (field == null)
            {
                throw new Exception($"Unable to find field {fieldName} in {enumType.FullName}");
            }
            var attribute = field.GetCustomAttribute<DisplayAsAttribute>();
            _values.Add(key, attribute == null ? fieldName : attribute.DisplayName);
        }
        
        _value = ResolveValue(defaultValue);
    }

    public void SetValue(TEnum value)
    {
        _value = value;
        DropdownSettingBehaviour?.SetValue(_values[_value]);
    }

    public override void CreateSettingUi(SettingsTabController settingsTabController)
    {
        DropdownSettingBehaviour = settingsTabController.CreateDropdownOption();
        DropdownSettingBehaviour.SetSetting(this);
        DropdownSettingBehaviour.SetTitle(Title);
        DropdownSettingBehaviour.SetOptions(_values.Select(x => x.Value).ToList());
        DropdownSettingBehaviour.SetValue(_values[_value]);
        
        DropdownSettingBehaviour.AddOnValueChangedListener(OnSettingBehaviourValueChanged);
    }
    
    public override void Save() => Save(_value.ToString());
    
    protected override void OnSettingBehaviourValueChanged()
    {
        _value = _values.ElementAt(DropdownSettingBehaviour.GetSettingValue()).Key;
        base.OnSettingBehaviourValueChanged();
        ValueChanged?.Invoke(_value);
    }

    public override SettingItem GetSettingBehaviour()
    {
        return DropdownSettingBehaviour;
    }

    public override void UiUpdate()
    {
        base.UiUpdate();
        var isEditable = IsEditable();
        DropdownSettingBehaviour.dropdown.interactable = isEditable;
    }

    protected override void SetValueFromMessageReader(MessageReader reader)
    {
        SetValue(Enum.Parse<TEnum>(reader.ReadString()));
    }
    protected override void WriteValueInMessageWriter(MessageWriter writer)
    {
        writer.Write(_value.ToString());
    }
}