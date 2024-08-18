using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using EnoPM.BetterVanilla.Data;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

public sealed class VanillaSettingsManager
{
    private readonly CustomSettingCategory _category;
    
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
    
    public VanillaSettingsManager(CustomSettingCategory category)
    {
        _category = category;
        
        ImpostorsCount = _category.Float("ImpostorsCount", "Impostors", new NumberRange(1f, 3f), stepSize: 1f, defaultValue: 1f);
        ImpostorsCount.ValueChanged += FloatValueChanged;
        ImpostorsKillCooldown = _category.Float("ImpostorsKillCooldown", "Kill Cooldown", new NumberRange(10f, 60f), stepSize: 0.5f, defaultValue: 25f, suffix: "s");
        ImpostorsKillCooldown.ValueChanged += FloatValueChanged;
        ImpostorsVision = _category.Float("ImpostorsVision", "Impostor Vision", new NumberRange(0.1f, 3f), stepSize: 0.05f, defaultValue: 1.25f, suffix: "x");
        ImpostorsVision.ValueChanged += FloatValueChanged;
        ImpostorsKillDistance = _category.Enum("ImpostorsKillDistance", "Kill Distance", defaultValue: ImpostorKillDistances.Short);
        ImpostorsKillDistance.ValueChanged += EnumValueChanged;
        PlayerSpeed = _category.Float("PlayerSpeed", "Player Speed", new NumberRange(0.1f, 3f), stepSize: 0.05f, defaultValue: 1f, suffix: "x");
        PlayerSpeed.ValueChanged += FloatValueChanged;
        CrewmateVision = _category.Float("CrewmateVision", "Crewmate Vision", new NumberRange(0.1f, 3f), stepSize: 0.05f, defaultValue: 0.5f, suffix: "x");
        CrewmateVision.ValueChanged += FloatValueChanged;
        EmergencyMeetingsCount = _category.Float("EmergencyMeetingsCount", "Emergency Meetings", new NumberRange(1f, 15f), stepSize: 1f, defaultValue: 1f);
        EmergencyMeetingsCount.ValueChanged += FloatValueChanged;
        EmergencyMeetingsCooldown = _category.Float("EmergencyMeetingsCooldown", "Emergency Cooldown", new NumberRange(0f, 120f), stepSize: 1f, defaultValue: 15f, suffix: "s");
        EmergencyMeetingsCooldown.ValueChanged += FloatValueChanged;
        DiscussionTime = _category.Float("DiscussionTime", "Discussion Time", new NumberRange(0f, 120f), stepSize: 1f, defaultValue: 0f, suffix: "s");
        DiscussionTime.ValueChanged += FloatValueChanged;
        VotingTime = _category.Float("VotingTime", "Voting Time", new NumberRange(0f, 300f), stepSize: 1f, defaultValue: 160f, suffix: "s");
        VotingTime.ValueChanged += FloatValueChanged;
        AnonymousVotes = _category.Bool("AnonymousVotes", "Anonymous Votes", true);
        AnonymousVotes.ValueChanged += BoolValueChanged;
        ConfirmEjects = _category.Bool("ConfirmEjects", "Confirm Ejects");
        ConfirmEjects.ValueChanged += BoolValueChanged;
        TaskBarUpdate = _category.Enum("TaskBarUpdate", "Task Bar Updates", defaultValue: TaskBarUpdates.Meetings);
        TaskBarUpdate.ValueChanged += EnumValueChanged;
        CommonTasks = _category.Float("CommonTasks", "Common Tasks", new NumberRange(1f, 4f), stepSize: 1f, defaultValue: 2f);
        CommonTasks.ValueChanged += FloatValueChanged;
        LongTasks = _category.Float("LongTasks", "Long Tasks", new NumberRange(1f, 6f), stepSize: 1f, defaultValue: 3f);
        LongTasks.ValueChanged += FloatValueChanged;
        ShortTasks = _category.Float("ShortTasks", "Short Tasks", new NumberRange(1f, 23f), stepSize: 1f, defaultValue: 5f);
        ShortTasks.ValueChanged += FloatValueChanged;
        VisualTasks = _category.Bool("VisualTasks", "Visual Tasks");
        VisualTasks.ValueChanged += BoolValueChanged;
    }

    private void FloatValueChanged(float _) => UpdateAllSettings();
    private void BoolValueChanged(bool _) => UpdateAllSettings();
    private void EnumValueChanged<TEnum>(TEnum _) where TEnum : struct => UpdateAllSettings();

    private void UpdateAllSettings()
    {
        var updates = new List<bool>
        {
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
            CheckAndUpdate(VisualTasks, BoolOptionNames.VisualTasks)
        };
        
        if (!updates.Contains(true)) return;
        SyncVanillaSettings();
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

    private static void SyncVanillaSettings()
    {
        GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
        GameManager.Instance.LogicOptions.SyncOptions();
    }

}