using System;
using System.Collections.Generic;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Data.Database;

namespace EnoPM.BetterVanilla.Core;

public abstract class CustomSetting
{
    public readonly string ID;
    protected readonly string Title;
    private readonly SaveTypes _saveType;
    private readonly Func<bool> _isEditableFunc;

    private SerializedPresetData Store => _saveType switch
    {
        SaveTypes.Local => DB.Presets.Current.Local,
        SaveTypes.HostToClient => DB.Presets.Current.Shared,
        _ => DB.Presets.Current.Shared
    };

    protected CustomSetting(string id, string title, SaveTypes saveType = SaveTypes.Local, Func<bool> isEditableFunc = null)
    {
        ID = id;
        Title = title;
        _saveType = saveType;
        _isEditableFunc = isEditableFunc;
    }

    public abstract void CreateSettingUi(SettingsTabController settingsTabController);
    public abstract void Save();
    public abstract SettingItem GetSettingBehaviour();

    protected virtual void OnSettingBehaviourValueChanged()
    {
        Save();
        DB.SavePresets();
    }

    public bool IsEditable() => _isEditableFunc?.Invoke() ?? true;

    protected void Save(float value)
    {
        Store.FloatStore[ID] = value;
    }

    protected void Save(bool value)
    {
        Store.BoolStore[ID] = value;
    }

    protected void Save(string value)
    {
        Store.StringStore[ID] = value;
    }

    protected float ResolveValue(float defaultValue) => Store.FloatStore.GetValueOrDefault(ID, defaultValue);
    protected bool ResolveValue(bool defaultValue) => Store.BoolStore.GetValueOrDefault(ID, defaultValue);
    protected string ResolveValue(string defaultValue) => Store.StringStore.GetValueOrDefault(ID, defaultValue);
    protected TEnum ResolveValue<TEnum>(TEnum defaultValue) where TEnum : struct
    {
        return Enum.Parse<TEnum>(ResolveValue(defaultValue.ToString()));
    }

    public enum SaveTypes
    {
        Local,
        HostToClient,
    }
}