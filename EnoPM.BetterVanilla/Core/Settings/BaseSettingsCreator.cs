using System;
using System.Collections.Generic;
using EnoPM.BetterVanilla.Core.Data;

namespace EnoPM.BetterVanilla.Core.Settings;

public abstract class BaseSettingsCreator
{
    public event Action<CustomSetting> SettingCreated;
    private Func<bool> _isEditableDefaultFunc;
    public readonly List<CustomSetting> Settings = [];

    protected BaseSettingsCreator(Func<bool> isEditableFunc = null)
    {
        _isEditableDefaultFunc = isEditableFunc;
        SettingCreated += OnSettingCreated;
    }

    protected virtual void OnSettingCreated(CustomSetting setting)
    {
        Settings.Add(setting);
    }

    protected void SetIsEditableDefaultFunc(Func<bool> isEditableFunc = null)
    {
        _isEditableDefaultFunc = isEditableFunc;
        foreach (var setting in Settings)
        {
            setting.SetIsEditableFunc(isEditableFunc);
        }
    }
    
    public virtual BoolSetting Bool(string id, string title, bool defaultValue = default, CustomSetting.SaveTypes saveType = CustomSetting.SaveTypes.Local, Func<bool> isEditableFunc = null)
    {
        var setting = new BoolSetting(id, title, defaultValue, saveType, isEditableFunc ?? _isEditableDefaultFunc);
        SettingCreated?.Invoke(setting);
        return setting;
    }
    
    public virtual FloatSetting Float(string id, string title, NumberRange range, float stepSize = 1f, string prefix = "", string suffix = "", float defaultValue = default, CustomSetting.SaveTypes saveType = CustomSetting.SaveTypes.Local, Func<bool> isEditableFunc = null)
    {
        var setting = new FloatSetting(id, title, range, stepSize, prefix, suffix, defaultValue, saveType, isEditableFunc ?? _isEditableDefaultFunc);
        SettingCreated?.Invoke(setting);
        return setting;
    }

    public virtual EnumSetting<TEnum> Enum<TEnum>(string id, string title, TEnum defaultValue = default, CustomSetting.SaveTypes saveType = CustomSetting.SaveTypes.Local, Func<bool> isEditableFunc = null) where TEnum : struct
    {
        var setting = new EnumSetting<TEnum>(id, title, defaultValue, saveType, isEditableFunc ?? _isEditableDefaultFunc);
        SettingCreated?.Invoke(setting);
        return setting;
    }
}