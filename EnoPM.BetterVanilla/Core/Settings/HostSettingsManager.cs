using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using EnoPM.BetterVanilla.Core.Data;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core.Settings;

public sealed class HostSettingsManager
{
    public readonly CustomSettingCategory Category;
    
    public readonly EnumSetting<AmongUsMaps> Map;
    public readonly FloatSetting ImpostorsCount;
    public readonly FloatSetting ImpostorsKillCooldown;
    public readonly FloatSetting ImpostorsVision;
    public readonly EnumSetting<ImpostorKillDistances> ImpostorsKillDistance;
    public readonly FloatSetting PlayerSpeed;
    public readonly FloatSetting CrewmateVision;
    public readonly FloatSetting EmergencyMeetingsCount;
    public readonly FloatSetting EmergencyMeetingsCooldown;
    public readonly FloatSetting DiscussionTime;
    public readonly FloatSetting VotingTime;
    public readonly BoolSetting AnonymousVotes;
    public readonly BoolSetting ConfirmEjects;
    public readonly EnumSetting<TaskBarUpdates> TaskBarUpdate;
    public readonly FloatSetting CommonTasks;
    public readonly FloatSetting LongTasks;
    public readonly FloatSetting ShortTasks;
    public readonly BoolSetting VisualTasks;
    
    public HostSettingsManager()
    {
        Category = new CustomSettingCategory("HostSettings", IsCategoryInteractable);
        
        Map = Category.Enum("Map", "Map", AmongUsMaps.Skeld);
        Map.ValueChanged += EnumValueChanged;
        
        ImpostorsCount = Category.Float("ImpostorsCount", "Impostors", new NumberRange(1f, 3f), stepSize: 1f, defaultValue: 1f);
        ImpostorsCount.ValueChanged += FloatValueChanged;
        
        ImpostorsKillCooldown = Category.Float("ImpostorsKillCooldown", "Kill Cooldown", new NumberRange(10f, 60f), stepSize: 0.5f, defaultValue: 25f, suffix: "s");
        ImpostorsKillCooldown.ValueChanged += FloatValueChanged;
        
        ImpostorsVision = Category.Float("ImpostorsVision", "Impostor Vision", new NumberRange(0.1f, 3f), stepSize: 0.05f, defaultValue: 1.25f, suffix: "x");
        ImpostorsVision.ValueChanged += FloatValueChanged;
        
        ImpostorsKillDistance = Category.Enum("ImpostorsKillDistance", "Kill Distance", defaultValue: ImpostorKillDistances.Short);
        ImpostorsKillDistance.ValueChanged += EnumValueChanged;
        
        PlayerSpeed = Category.Float("PlayerSpeed", "Player Speed", new NumberRange(0.1f, 3f), stepSize: 0.05f, defaultValue: 1f, suffix: "x");
        PlayerSpeed.ValueChanged += FloatValueChanged;
        
        CrewmateVision = Category.Float("CrewmateVision", "Crewmate Vision", new NumberRange(0.1f, 3f), stepSize: 0.05f, defaultValue: 0.5f, suffix: "x");
        CrewmateVision.ValueChanged += FloatValueChanged;
        
        EmergencyMeetingsCount = Category.Float("EmergencyMeetingsCount", "Emergency Meetings", new NumberRange(1f, 15f), stepSize: 1f, defaultValue: 1f);
        EmergencyMeetingsCount.ValueChanged += FloatValueChanged;
        
        EmergencyMeetingsCooldown = Category.Float("EmergencyMeetingsCooldown", "Emergency Cooldown", new NumberRange(0f, 120f), stepSize: 1f, defaultValue: 15f, suffix: "s");
        EmergencyMeetingsCooldown.ValueChanged += FloatValueChanged;
        
        DiscussionTime = Category.Float("DiscussionTime", "Discussion Time", new NumberRange(0f, 120f), stepSize: 1f, defaultValue: 0f, suffix: "s");
        DiscussionTime.ValueChanged += FloatValueChanged;
        
        VotingTime = Category.Float("VotingTime", "Voting Time", new NumberRange(0f, 300f), stepSize: 1f, defaultValue: 160f, suffix: "s");
        VotingTime.ValueChanged += FloatValueChanged;
        
        AnonymousVotes = Category.Bool("AnonymousVotes", "Anonymous Votes", true);
        AnonymousVotes.ValueChanged += BoolValueChanged;
        
        ConfirmEjects = Category.Bool("ConfirmEjects", "Confirm Ejects");
        ConfirmEjects.ValueChanged += BoolValueChanged;
        
        TaskBarUpdate = Category.Enum("TaskBarUpdate", "Task Bar Updates", defaultValue: TaskBarUpdates.Meetings);
        TaskBarUpdate.ValueChanged += EnumValueChanged;
        
        CommonTasks = Category.Float("CommonTasks", "Common Tasks", new NumberRange(1f, 4f), stepSize: 1f, defaultValue: 2f);
        CommonTasks.ValueChanged += FloatValueChanged;
        
        LongTasks = Category.Float("LongTasks", "Long Tasks", new NumberRange(1f, 6f), stepSize: 1f, defaultValue: 3f);
        LongTasks.ValueChanged += FloatValueChanged;
        
        ShortTasks = Category.Float("ShortTasks", "Short Tasks", new NumberRange(1f, 23f), stepSize: 1f, defaultValue: 5f);
        ShortTasks.ValueChanged += FloatValueChanged;
        
        VisualTasks = Category.Bool("VisualTasks", "Visual Tasks");
        VisualTasks.ValueChanged += BoolValueChanged;
    }

    private bool IsCategoryInteractable()
    {
        return AmongUsClient.Instance && AmongUsClient.Instance.AmHost;
    }

    private void FloatValueChanged(float _) => UpdateAllSettingsAndSync();
    private void BoolValueChanged(bool _) => UpdateAllSettingsAndSync();
    private void EnumValueChanged<TEnum>(TEnum _) where TEnum : struct => UpdateAllSettingsAndSync();

    private void UpdateAllSettingsAndSync()
    {
        if (UpdateAllSettings() && Utils.IsHost(PlayerControl.LocalPlayer))
        {
            GameManager.Instance.LogicOptions.SyncOptions();
        }
    }

    public bool UpdateAllSettings()
    {
        var updated = new List<bool>
        {
            CheckAndUpdate(Map, ByteOptionNames.MapId),
            CheckAndUpdate(ImpostorsCount, Int32OptionNames.NumImpostors),
            CheckAndUpdate(ImpostorsKillCooldown, FloatOptionNames.KillCooldown),
            CheckAndUpdate(ImpostorsVision, FloatOptionNames.ImpostorLightMod),
            CheckAndUpdate(ImpostorsKillDistance, Int32OptionNames.KillDistance),
            CheckAndUpdate(PlayerSpeed, FloatOptionNames.PlayerSpeedMod),
            CheckAndUpdate(CrewmateVision, FloatOptionNames.CrewLightMod),
            CheckAndUpdate(EmergencyMeetingsCount, Int32OptionNames.NumEmergencyMeetings),
            CheckAndUpdate(EmergencyMeetingsCooldown, Int32OptionNames.EmergencyCooldown),
            CheckAndUpdate(DiscussionTime, Int32OptionNames.DiscussionTime),
            CheckAndUpdate(VotingTime, Int32OptionNames.VotingTime),
            CheckAndUpdate(AnonymousVotes, BoolOptionNames.AnonymousVotes),
            CheckAndUpdate(ConfirmEjects, BoolOptionNames.ConfirmImpostor),
            CheckAndUpdate(TaskBarUpdate, Int32OptionNames.TaskBarMode),
            CheckAndUpdate(CommonTasks, Int32OptionNames.NumCommonTasks),
            CheckAndUpdate(LongTasks, Int32OptionNames.NumLongTasks),
            CheckAndUpdate(ShortTasks, Int32OptionNames.NumShortTasks),
            CheckAndUpdate(VisualTasks, BoolOptionNames.VisualTasks),
            CheckAndUpdate((int)RulesPresets.Custom, Int32OptionNames.RulePreset)
        }.Contains(true);

        if (!updated) return false;
        
        GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
        GameOptionsManager.Instance.SaveNormalHostOptions();

        return true;
    }

    public void OnUpdatedByVanillaUi(NumberOption option)
    {
        if (option.floatOptionName != FloatOptionNames.Invalid)
        {
            VanillaOptionUpdate(option.floatOptionName, option.GetFloat());
        }
        else if (option.intOptionName != Int32OptionNames.Invalid)
        {
            VanillaOptionUpdate(option.intOptionName, option.GetInt());
        }
    }

    public void OnUpdatedByVanillaUi(ToggleOption option)
    {
        VanillaOptionUpdate(option.boolOptionName, option.GetBool());
    }

    private void VanillaOptionUpdate(BoolOptionNames optionName, bool value)
    {
        switch (optionName)
        {
            case BoolOptionNames.AnonymousVotes:
                AnonymousVotes.SetValue(value);
                break;
            case BoolOptionNames.ConfirmImpostor:
                ConfirmEjects.SetValue(value);
                break;
            case BoolOptionNames.VisualTasks:
                VisualTasks.SetValue(value);
                break;
        }
    }

    private void VanillaOptionUpdate(Int32OptionNames optionName, int value)
    {
        switch (optionName)
        {
            case Int32OptionNames.NumImpostors:
                ImpostorsCount.SetValue(value);
                break;
            case Int32OptionNames.NumEmergencyMeetings:
                EmergencyMeetingsCount.SetValue(value);
                break;
            case Int32OptionNames.EmergencyCooldown:
                EmergencyMeetingsCooldown.SetValue(value);
                break;
            case Int32OptionNames.DiscussionTime:
                DiscussionTime.SetValue(value);
                break;
            case Int32OptionNames.VotingTime:
                VotingTime.SetValue(value);
                break;
            case Int32OptionNames.NumCommonTasks:
                CommonTasks.SetValue(value);
                break;
            case Int32OptionNames.NumLongTasks:
                LongTasks.SetValue(value);
                break;
            case Int32OptionNames.NumShortTasks:
                ShortTasks.SetValue(value);
                break;
            case Int32OptionNames.KillDistance:
                ImpostorsKillDistance.SetValue((ImpostorKillDistances)value);
                break;
            case Int32OptionNames.TaskBarMode:
                TaskBarUpdate.SetValue((TaskBarUpdates)value);
                break;
        }
    }

    private void VanillaOptionUpdate(FloatOptionNames optionName, float value)
    {
        switch (optionName)
        {
            case FloatOptionNames.KillCooldown:
                ImpostorsKillCooldown.SetValue(value);
                break;
            case FloatOptionNames.ImpostorLightMod:
                ImpostorsVision.SetValue(value);
                break;
            case FloatOptionNames.PlayerSpeedMod:
                PlayerSpeed.SetValue(value);
                break;
            case FloatOptionNames.CrewLightMod:
                CrewmateVision.SetValue(value);
                break;
        }
    }
    
    private static bool CheckAndUpdate(BoolSetting setting, BoolOptionNames optName)
    {
        var currentValue = GameOptionsManager.Instance.CurrentGameOptions.GetBool(optName);
        var newValue = (bool)setting;

        if (currentValue == newValue)
        {
            return false;
        }
        
        GameOptionsManager.Instance.CurrentGameOptions.SetBool(optName, newValue);

        return true;
    }
    
    private static bool CheckAndUpdate(FloatSetting setting, FloatOptionNames optName)
    {
        var currentValue = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(optName);
        var newValue = (float)setting;

        if (Mathf.Approximately(currentValue, newValue))
        {
            return false;
        }
        
        GameOptionsManager.Instance.CurrentGameOptions.SetFloat(optName, newValue);

        return true;
    }

    private static bool CheckAndUpdate(int newValue, Int32OptionNames optName)
    {
        var currentValue = GameOptionsManager.Instance.CurrentGameOptions.GetInt(optName);

        if (currentValue == newValue)
        {
            return false;
        }
        
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(optName, newValue);

        return true;
    }

    private static bool CheckAndUpdate(FloatSetting setting, Int32OptionNames optName)
    {
        var currentValue = GameOptionsManager.Instance.CurrentGameOptions.GetInt(optName);
        var newValue = (int)setting;

        if (currentValue == newValue)
        {
            return false;
        }
        
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(optName, newValue);

        return true;
    }
    
    private static bool CheckAndUpdate<TEnum>(EnumSetting<TEnum> setting, Int32OptionNames optName) where TEnum : struct
    {
        var currentValue = GameOptionsManager.Instance.CurrentGameOptions.GetInt(optName);
        var newValue = Convert.ToInt32((TEnum)setting);

        if (currentValue == newValue)
        {
            return false;
        }
        
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(optName, newValue);

        return true;
    }
    
    private static bool CheckAndUpdate<TEnum>(EnumSetting<TEnum> setting, ByteOptionNames optName) where TEnum : struct
    {
        var currentValue = GameOptionsManager.Instance.CurrentGameOptions.GetByte(optName);
        var newValue = Convert.ToByte((TEnum)setting);

        if (currentValue == newValue)
        {
            return false;
        }
        
        GameOptionsManager.Instance.CurrentGameOptions.SetByte(optName, newValue);

        return true;
    }

}