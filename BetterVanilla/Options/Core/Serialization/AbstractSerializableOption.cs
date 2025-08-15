using System;
using System.IO;
using BetterVanilla.Components;
using Hazel;

namespace BetterVanilla.Options.Core.Serialization;

public abstract class AbstractSerializableOption(string key, string title)
{
    public string Key { get; } = key;
    public string Title { get; } = title;
    private Func<bool>? IsLockedFunc { get; set; }
    private Func<bool>? IsHiddenFunc { get; set; }
    public string? LockedText { get; private set; }
    
    public event Action? ValueChanged;

    protected void TriggerValueChanged() => ValueChanged?.Invoke();
    
    public abstract string GetValueAsString();
    public abstract void WriteValue(MessageWriter writer);
    public abstract void ReadValue(MessageReader reader);
    public abstract void WriteValue(BinaryWriter writer);
    public abstract void ReadValue(BinaryReader reader);

    public bool IsLocked() => IsLockedFunc != null && IsLockedFunc();
    public bool IsHidden() => IsHiddenFunc != null && IsHiddenFunc();
    public bool IsNotAllowed() => IsLocked() || IsHidden();
    public bool IsAllowed() => !IsNotAllowed();

    public virtual void UpdateBehaviour()
    {
        
    }

    public void SetIsLockedFunc(Func<bool>? isLockedFunc)
    {
        IsLockedFunc = isLockedFunc;
    }

    public void SetIsHiddenFunc(Func<bool>? isHiddenFunc)
    {
        IsHiddenFunc = isHiddenFunc;
    }

    public void SetLockedText(string? lockedText)
    {
        LockedText = lockedText;
    }
}