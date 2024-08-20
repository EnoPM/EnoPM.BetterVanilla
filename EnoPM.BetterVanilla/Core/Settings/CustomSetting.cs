using System;
using System.Collections.Generic;
using System.Linq;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core.Data.Database;
using Hazel;

namespace EnoPM.BetterVanilla.Core.Settings;

public abstract class CustomSetting
{
    private static readonly List<CustomSetting> AllSettings = [];

    public static void CheckLockedSettings(string categoryId)
    {
        var category = CustomSettingCategory.GetCategory(categoryId);
        if (category == null) return;
        foreach (var setting in category.Settings)
        {
            var behaviour = setting.GetSettingBehaviour();
            if (!behaviour) return;
            var isLocked = setting.IsLocked();
            behaviour.gameObject.SetActive(!isLocked);
        }
    }

    public static void HandleRpcShareHostToClientSettingChange(PlayerControl sender, MessageReader reader)
    {
        var id = reader.ReadString();
        var setting = AllSettings.Find(x => x.ID == id);
        if (setting == null)
        {
            Plugin.Logger.LogWarning($"Unable to find setting by id: {id} in HandleRpcShareHostToClientSettingChange");
            return;
        }
        setting.SetValueFromMessageReader(reader);
    }

    public static void HandleRpcBulkShareHostToClientSettingChange(PlayerControl sender, MessageReader reader)
    {
        var size = reader.ReadInt32();
        for (var i = 0; i < size; i++)
        {
            var id = reader.ReadString();
            var setting = AllSettings.Find(x => x.ID == id);
            if (setting == null)
            {
                Plugin.Logger.LogError($"Unable to find setting by id: {id} in HandleRpcBulkShareHostToClientSettingChange");
                return;
            }
            setting.SetValueFromMessageReader(reader);
        }
    }

    public static void WriteAllHostToClientSettings(MessageWriter writer)
    {
        var allSettings = AllSettings.Where(x => x._saveType == SaveTypes.HostToClient).ToList();
        writer.Write(allSettings.Count);
        foreach (var setting in allSettings)
        {
            writer.Write(setting.ID);
            setting.WriteValueInMessageWriter(writer);
        }
    }

    public readonly string ID;
    protected readonly string Title;
    private readonly SaveTypes _saveType;
    private Func<bool> _isEditableFunc;
    private Func<bool> _isLockedFunc;

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

        AllSettings.Add(this);
    }

    public abstract void CreateSettingUi(SettingsTabController settingsTabController);
    public abstract void Save();
    public abstract SettingItem GetSettingBehaviour();

    protected abstract void SetValueFromMessageReader(MessageReader reader);
    protected abstract void WriteValueInMessageWriter(MessageWriter writer);

    protected virtual void OnSettingBehaviourValueChanged()
    {
        Save();
        DB.SavePresets();
    }

    protected bool IsEditable() => _isEditableFunc?.Invoke() ?? true;
    public bool IsLocked() => _isLockedFunc?.Invoke() ?? false;

    public void SetIsEditableFunc(Func<bool> isEditableFunc)
    {
        _isEditableFunc = isEditableFunc;
    }

    public void SetIsLockedFunc(Func<bool> isLockedFunc)
    {
        _isLockedFunc = isLockedFunc;
    }

    protected void Save(float value)
    {
        Store.FloatStore[ID] = value;
        if (_saveType == SaveTypes.HostToClient && AmongUsClient.Instance && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.RpcShareHostToClientSettingChange(ID, value);
        }
    }

    protected void Save(bool value)
    {
        Store.BoolStore[ID] = value;
        if (_saveType == SaveTypes.HostToClient && AmongUsClient.Instance && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.RpcShareHostToClientSettingChange(ID, value);
        }
    }

    protected void Save(string value)
    {
        Store.StringStore[ID] = value;
        if (_saveType == SaveTypes.HostToClient && AmongUsClient.Instance && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.RpcShareHostToClientSettingChange(ID, value);
        }
    }

    protected float ResolveValue(float defaultValue) => Store.FloatStore.GetValueOrDefault(ID, defaultValue);
    protected bool ResolveValue(bool defaultValue) => Store.BoolStore.GetValueOrDefault(ID, defaultValue);
    protected string ResolveValue(string defaultValue) => Store.StringStore.GetValueOrDefault(ID, defaultValue);
    protected TEnum ResolveValue<TEnum>(TEnum defaultValue) where TEnum : struct
    {
        return Enum.Parse<TEnum>(ResolveValue(defaultValue.ToString()));
    }

    public virtual void UiUpdate()
    {

    }

    public enum SaveTypes
    {
        Local,
        HostToClient,
    }
}