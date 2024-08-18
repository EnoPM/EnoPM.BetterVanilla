using System;
using System.Collections.Generic;
using EnoPM.BetterVanilla.Data;

namespace EnoPM.BetterVanilla.Core;

public sealed class CustomSettingCategory
{
    public static readonly List<CustomSettingCategory> AllCategories = [];
    public static CustomSettingCategory GetCategory(string id) => AllCategories.Find(x => x.Id == id);
    
    public readonly string Id;
    public readonly List<CustomSetting> Settings = [];
    private readonly Func<bool> _isEditableFunc;
    
    public CustomSettingCategory(string id, Func<bool> isEditableFunc = null)
    {
        Id = id;
        _isEditableFunc = isEditableFunc;
        AllCategories.Add(this);
    }

    public void RefreshEditableState()
    {
        foreach (var setting in Settings)
        {
            var behaviour = setting.GetSettingBehaviour();
            behaviour?.SetEditable(setting.IsEditable());
        }
    }

    public BoolSetting Bool(string id, string title, bool defaultValue = default, CustomSetting.SaveTypes saveType = CustomSetting.SaveTypes.Local)
    {
        var setting = new BoolSetting(id, title, defaultValue, saveType, _isEditableFunc);
        Settings.Add(setting);
        return setting;
    }

    public FloatSetting Float(string id, string title, NumberRange range, float stepSize = 1f, string prefix = "", string suffix = "", float defaultValue = default, CustomSetting.SaveTypes saveType = CustomSetting.SaveTypes.Local)
    {
        var setting = new FloatSetting(id, title, range, stepSize, prefix, suffix, defaultValue, saveType, _isEditableFunc);
        Settings.Add(setting);
        return setting;
    }

    public EnumSetting<TEnum> Enum<TEnum>(string id, string title, TEnum defaultValue = default, CustomSetting.SaveTypes saveType = CustomSetting.SaveTypes.Local) where TEnum : struct
    {
        var setting = new EnumSetting<TEnum>(id, title, defaultValue, saveType, _isEditableFunc);
        Settings.Add(setting);
        return setting;
    }
}