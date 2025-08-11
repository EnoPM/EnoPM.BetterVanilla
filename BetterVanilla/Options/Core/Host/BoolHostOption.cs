using AmongUs.GameOptions;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Options.Core.Host;

public sealed class BoolHostOption : BoolSerializableOption, IHostOption<CheckboxGameSetting, ToggleOption>
{
    public CheckboxGameSetting GameSetting { get; }
    public ToggleOption? SettingBehaviour { get; private set; }
    public ViewSettingsInfoPanel? ViewSettingBehaviour { get; private set; }
    
    public BoolHostOption(string key, string title, bool defaultValue) : base(key, title, defaultValue)
    {
        GameSetting = ScriptableObject.CreateInstance<CheckboxGameSetting>();
        GameSetting.Type = OptionTypes.Checkbox;
        GameSetting.hideFlags = HideFlags.DontSave;
        GameSetting.name = $"BetterVanillaCustomSetting_{key}";
        GameSetting.Title = StringNames.None;
        GameSetting.OptionName = BoolOptionNames.Invalid;
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

    public void OnBehaviourCreated(ToggleOption behaviour)
    {
        SettingBehaviour = behaviour;
        SettingBehaviour.TitleText.SetText(Title);
        SettingBehaviour.CheckMark.enabled = Value;
    }

    public void OnViewBehaviourCreated(ViewSettingsInfoPanel viewBehaviour)
    {
        ViewSettingBehaviour = viewBehaviour;
    }

    public void UpdateValueFromSettingBehaviour()
    {
        if (SettingBehaviour == null) return;
        Value = SettingBehaviour.GetBool();
    }

    public void UpdateBehaviours()
    {
        if (SettingBehaviour != null)
        {
            SettingBehaviour.CheckMark.enabled = Value;
        }
        if (ViewSettingBehaviour != null)
        {
            SetViewSettingBehaviourInfo(ViewSettingBehaviour, 61);
        }
    }

    public void SetViewSettingBehaviourInfo(ViewSettingsInfoPanel viewBehaviour, int maskLayer)
    {
        viewBehaviour.titleText.SetText(Title);
        viewBehaviour.settingText.SetText(string.Empty);
        viewBehaviour.checkMarkOff.gameObject.SetActive(!Value);
        viewBehaviour.background.gameObject.SetActive(true);
        viewBehaviour.checkMark.gameObject.SetActive(Value);
        viewBehaviour.SetMaskLayer(maskLayer);
    }

}