using AmongUs.GameOptions;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Options.Core.Host;

public sealed class NumberHostOption : NumberSerializableOption, IHostOption<FloatGameSetting, NumberOption>
{
    public FloatGameSetting GameSetting { get; }
    public NumberOption? SettingBehaviour { get; private set; }
    public ViewSettingsInfoPanel? ViewSettingBehaviour { get; private set; }

    public NumberHostOption(string key, string title, float defaultValue, float incrementValue, float minValue, float maxValue, string valuePrefix = "", string valueSuffix = "") : base(key, title, defaultValue, incrementValue, minValue, maxValue, valuePrefix, valueSuffix)
    {
        GameSetting = ScriptableObject.CreateInstance<FloatGameSetting>();
        GameSetting.Type = OptionTypes.Float;
        GameSetting.hideFlags = HideFlags.DontSave;
        GameSetting.name = $"BetterVanillaCustomSetting_{key}";
        GameSetting.Title = StringNames.None;
        GameSetting.Value = Value;
        GameSetting.OptionName = FloatOptionNames.Invalid;
        GameSetting.Increment = IncrementValue;
        GameSetting.FormatString = "0.00";
        GameSetting.ZeroIsInfinity = false;
        GameSetting.SuffixType = NumberSuffixes.Seconds;
        GameSetting.ValidRange = new FloatRange(MinValue, MaxValue);
        ValueChanged += OnValueChanged;
    }

    private void OnValueChanged()
    {
        if (LocalConditions.AmHost())
        {
            PlayerControl.LocalPlayer.RpcShareHostOption(this);
        }
        UpdateBehaviours();
    }
    
    public BaseGameSetting GetGameSetting() => GameSetting;
    public OptionBehaviour? GetOptionBehaviour() => SettingBehaviour;
    
    public void OnBehaviourCreated(NumberOption behaviour)
    {
        SettingBehaviour = behaviour;
        behaviour.TitleText.SetText(Title);
        behaviour.Value = Value;
        behaviour.AdjustButtonsActiveState();
    }
    
    public void OnViewBehaviourCreated(ViewSettingsInfoPanel viewBehaviour)
    {
        ViewSettingBehaviour = viewBehaviour;
    }
    
    public void UpdateValueFromSettingBehaviour()
    {
        if (SettingBehaviour == null) return;
        Value = SettingBehaviour.GetFloat();
    }
    
    public void UpdateBehaviours()
    {
        if (SettingBehaviour != null)
        {
            SettingBehaviour.Value = Value;
            SettingBehaviour.AdjustButtonsActiveState();
        }
        if (ViewSettingBehaviour != null)
        {
            SetViewSettingBehaviourInfo(ViewSettingBehaviour, 61);
        }
    }
    
    public void SetViewSettingBehaviourInfo(ViewSettingsInfoPanel viewBehaviour, int maskLayer)
    {
        viewBehaviour.titleText.SetText(Title);
        viewBehaviour.settingText.SetText(GetValueAsString());
        viewBehaviour.disabledBackground.gameObject.SetActive(false);
        viewBehaviour.background.gameObject.SetActive(true);
        viewBehaviour.SetMaskLayer(maskLayer);
    }
}