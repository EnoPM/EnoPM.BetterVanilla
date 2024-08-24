using Hazel;

namespace BetterVanilla.Core.Options;

public sealed class BoolLocalOption : BaseLocalOption
{
    private bool _value;
    public bool Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            _value = value;
            OnValueChanged();
        }
    }
    
    public BoolLocalOption(string name, string title, bool defaultValue) : base(name, title)
    {
        _value = LoadValueFromDatabase(defaultValue);
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