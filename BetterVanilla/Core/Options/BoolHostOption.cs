using System;
using AmongUs.GameOptions;
using BetterVanilla.Core.Extensions;
using Hazel;

namespace BetterVanilla.Core.Options;

public class BoolHostOption : BaseHostOption
{
    private bool _value;
    private bool Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            _value = value;
            OnValueChanged();
        }
    }
    
    public BoolHostOption(string name, string title, bool defaultValue) : base(name, title)
    {
        var setting = InitGameSetting<CheckboxGameSetting>(OptionTypes.Checkbox);
        setting.OptionName = BoolOptionNames.Invalid;
        _value = LoadValueFromDatabase(defaultValue);
    }

    public override bool GetBool()
    {
        return Value;
    }

    public override void OnBehaviourCreated(OptionBehaviour behaviour)
    {
        base.OnBehaviourCreated(behaviour);
        if (behaviour is not ToggleOption toggleOption)
        {
            throw new ArgumentException($"behaviour must be {nameof(ToggleOption)}", nameof(behaviour));
        }
        toggleOption.TitleText.SetText(Title);
        toggleOption.CheckMark.enabled = Value;
    }
    
    public override void UpdateValueFromBehaviour()
    {
        Value = Behaviour.GetBool();
    }
    
    protected override void UpdateBehaviourValue()
    {
        if (Behaviour)
        {
            var toggleOption = Behaviour.TryCast<ToggleOption>();
            if (toggleOption == null)
            {
                throw new ArgumentException($"behaviour must be {nameof(ToggleOption)}");
            }
            toggleOption.CheckMark.enabled = Value;
        }
        if (ViewBehaviour)
        {
            ViewBehaviour.CustomSetInfoCheckbox(61, this);
        }
    }

    public override void WriteValue(MessageWriter messageWriter)
    {
        messageWriter.Write(Value);
    }
    
    public override void ReadValue(MessageReader messageReader)
    {
        Value = messageReader.ReadBoolean();
    }
    public override string GetValueString()
    {
        return TranslationController.Instance.GetString(Value ? StringNames.SettingsOn : StringNames.SettingsOff);
    }
    
    protected override void OnValueChanged()
    {
        base.OnValueChanged();
        SaveValueInDatabase(Value);
    }
}