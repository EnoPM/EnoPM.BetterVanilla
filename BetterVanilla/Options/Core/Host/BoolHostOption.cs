using AmongUs.GameOptions;
using BetterVanilla.Components;
using BetterVanilla.Core;
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
        if (LocalConditions.AmHost() && BetterPlayerControl.LocalPlayer != null)
        {
            BetterPlayerControl.LocalPlayer.RpcSetHostOptionValue(this);
        }
        UpdateBehaviours();
    }
    
    public BaseGameSetting GetGameSetting() => GameSetting;
    public OptionBehaviour? GetOptionBehaviour() => SettingBehaviour;

    public void OnBehaviourCreated(ToggleOption behaviour)
    {
        SettingBehaviour = behaviour;
        SettingBehaviour.TitleText.SetText(Title);
        SettingBehaviour.TitleText.color = Color.magenta;
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
        var allowed = IsAllowed();
        var color = allowed ? Palette.EnabledColor : Palette.DisabledClear;
        viewBehaviour.titleText.SetText(Title);
        viewBehaviour.titleText.color = Color.magenta;
        viewBehaviour.settingText.SetText(string.Empty);
        viewBehaviour.checkMarkOff.gameObject.SetActive(!Value);
        viewBehaviour.background.gameObject.SetActive(true);
        viewBehaviour.checkMark.gameObject.SetActive(Value);
        if (viewBehaviour.background.color != color)
        {
            viewBehaviour.background.color = color;
        }
        if (viewBehaviour.checkMarkOff.color != color)
        {
            viewBehaviour.checkMarkOff.color = color;
        }
        if (viewBehaviour.checkMark.color != color)
        {
            viewBehaviour.checkMark.color = color;
        }
        viewBehaviour.SetMaskLayer(maskLayer);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        if (SettingBehaviour == null) return;
        var interactable = IsAllowed();
        var color = interactable ? Palette.EnabledColor : Palette.DisabledClear;
        if (SettingBehaviour.CheckMark.color != color)
        {
            SettingBehaviour.CheckMark.color = color;
        }
        SettingBehaviour.enabled = interactable;
    }

}