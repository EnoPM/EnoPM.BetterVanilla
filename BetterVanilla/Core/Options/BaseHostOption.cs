using System;
using System.Collections.Generic;
using BetterVanilla.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Core.Options;

public abstract class BaseHostOption : BaseOption
{
    public static readonly List<BaseHostOption> AllOptions = [];
    public BaseGameSetting GameSetting { get; private set; }
    public OptionBehaviour Behaviour { get; private set; }
    public ViewSettingsInfoPanel ViewBehaviour { get; private set; }

    protected BaseHostOption(string name, string title) : base(name, title)
    {
        AllOptions.Add(this);
    }

    public virtual float GetFloat() => throw new NotImplementedException();
    public virtual int GetInt() => throw new NotImplementedException();
    public virtual bool GetBool() => throw new NotImplementedException();

    protected TGameSetting InitGameSetting<TGameSetting>(OptionTypes optionType) where TGameSetting : BaseGameSetting
    {
        var setting = ScriptableObject.CreateInstance<TGameSetting>();
        setting.hideFlags = HideFlags.DontSave;
        setting.name = "BetterVanillaCustomSetting";
        setting.Title = StringNames.None;
        setting.Type = optionType;
        GameSetting = setting;
        return setting;
    }

    public virtual void OnBehaviourCreated(OptionBehaviour behaviour)
    {
        Behaviour = behaviour;
    }

    public virtual void OnViewBehaviourCreated(ViewSettingsInfoPanel viewBehaviour)
    {
        ViewBehaviour = viewBehaviour;
    }

    protected virtual void OnValueChanged()
    {
        if (AmongUsClient.Instance && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.RpcShareHostOption(this);
        }
        UpdateBehaviourValue();
    }

    public abstract void UpdateValueFromBehaviour();
    protected abstract void UpdateBehaviourValue();
}