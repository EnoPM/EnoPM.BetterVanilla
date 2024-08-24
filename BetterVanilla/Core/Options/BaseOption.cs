using BetterVanilla.Components;
using Hazel;

namespace BetterVanilla.Core.Options;

public abstract class BaseOption
{
    public readonly string Name;
    public string Title { get; protected set; }

    protected BaseOption(string name, string title)
    {
        Name = name;
        Title = title;
    }

    public void WriteIn(MessageWriter writer)
    {
        writer.Write(Name);
        WriteValue(writer);
    }
    
    public abstract void WriteValue(MessageWriter messageWriter);
    public abstract void ReadValue(MessageReader messageReader);
    public abstract string GetValueString();

    protected bool LoadValueFromDatabase(bool defaultValue) => BetterVanillaManager.Instance.Database.Data.CurrentPreset.GetValueOrDefault(Name, defaultValue);
    protected string LoadValueFromDatabase(string defaultValue) => BetterVanillaManager.Instance.Database.Data.CurrentPreset.GetValueOrDefault(Name, defaultValue);
    protected int LoadValueFromDatabase(int defaultValue) => BetterVanillaManager.Instance.Database.Data.CurrentPreset.GetValueOrDefault(Name, defaultValue);
    protected float LoadValueFromDatabase(float defaultValue) => BetterVanillaManager.Instance.Database.Data.CurrentPreset.GetValueOrDefault(Name, defaultValue);
    protected TEnum LoadValueFromDatabase<TEnum>(TEnum defaultValue) where TEnum: struct => BetterVanillaManager.Instance.Database.Data.CurrentPreset.GetValueOrDefault(Name, defaultValue);

    protected void SaveValueInDatabase(bool value)
    {
        BetterVanillaManager.Instance.Database.Data.CurrentPreset.BoolStore[Name] = value;
        BetterVanillaManager.Instance.Database.Save();
    }
    
    protected void SaveValueInDatabase(string value)
    {
        BetterVanillaManager.Instance.Database.Data.CurrentPreset.StringStore[Name] = value;
        BetterVanillaManager.Instance.Database.Save();
    }
    
    protected void SaveValueInDatabase(int value)
    {
        BetterVanillaManager.Instance.Database.Data.CurrentPreset.IntStore[Name] = value;
        BetterVanillaManager.Instance.Database.Save();
    }
    
    protected void SaveValueInDatabase(float value)
    {
        BetterVanillaManager.Instance.Database.Data.CurrentPreset.FloatStore[Name] = value;
        BetterVanillaManager.Instance.Database.Save();
    }

    protected void SaveValueInDatabase<TEnum>(TEnum value) where TEnum : struct
    {
        BetterVanillaManager.Instance.Database.Data.CurrentPreset.StringStore[Name] = value.ToString();
        BetterVanillaManager.Instance.Database.Save();
    }
}