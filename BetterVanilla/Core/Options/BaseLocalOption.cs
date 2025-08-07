using System;
using System.Collections.Generic;
using BetterVanilla.Components;
using BetterVanilla.Components.BaseComponents;

namespace BetterVanilla.Core.Options;

public abstract class BaseLocalOption : BaseOption
{
    public static readonly List<BaseLocalOption> AllOptions = [];
    
    private string _lockHash;

    public event Action? ValueChanged;
    
    public BaseSettingBehaviour Behaviour { get; private set; }

    protected BaseLocalOption(string name, string title) : base(name, title)
    {
        AllOptions.Add(this);
    }

    public virtual void OnBehaviourCreated(BaseSettingBehaviour behaviour)
    {
        Behaviour = behaviour;
    }

    public void LockWithPassword(string hash)
    {
        FeatureCodeBehaviour.Instance?.RegisterHash(hash);
        _lockHash = hash;
    }

    public bool IsLocked()
    {
        return !string.IsNullOrEmpty(_lockHash) && FeatureCodeBehaviour.Instance?.IsUnlocked(_lockHash) == false;
    }

    protected virtual void OnValueChanged()
    {
        Behaviour?.UpdateFromOption();
        ValueChanged?.Invoke();
    }
}