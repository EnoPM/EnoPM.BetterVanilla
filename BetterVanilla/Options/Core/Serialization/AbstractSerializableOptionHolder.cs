using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Options.Core.Local;
using Rewired;

namespace BetterVanilla.Options.Core.Serialization;

public abstract partial class AbstractSerializableOptionHolder
{
    private static readonly string OptionsDirectory;

    static AbstractSerializableOptionHolder()
    {
        OptionsDirectory = Path.Combine(
            ModPaths.ModDataDirectory,
            "Options"
        );

        if (Directory.Exists(OptionsDirectory)) return;
        Directory.CreateDirectory(OptionsDirectory);
    }

    private string FilePath { get; }
    private Dictionary<string, AbstractSerializableOption> Options { get; } = [];
    private CancellationTokenSource? DebounceToken { get; set; }

    protected int DebounceDelayInSeconds { get; set; } = 5;

    protected AbstractSerializableOptionHolder(string filename)
    {
        FilePath = Path.Combine(OptionsDirectory, filename);

        InitOptionProperties();
        LoadOptionValuesFromFile();
    }

    public AbstractSerializableOption[] GetOptions()
    {
        return Options.Select(x => x.Value).ToArray();
    }

    public void Save() => SaveOptionValuesToFile();

    protected void ValueChangedHandler()
    {
        DebounceToken?.Cancel();
        DebounceToken = new CancellationTokenSource();
        var token = DebounceToken.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(DebounceDelayInSeconds), token);
                if (!token.IsCancellationRequested)
                {
                    SaveOptionValuesToFile();
                }
            }
            catch (TaskCanceledException)
            {
                // ignored - debounce canceled
            }
        }, token);
    }

    private void SaveOptionValuesToFile()
    {
        Ls.LogInfo($"Saving options to file: {FilePath}");
        using var file = File.Create(FilePath);
        using var writer = new BinaryWriter(file);

        writer.Write(Options.Count);

        foreach (var option in Options.Values)
        {
            writer.Write(option.Key);
            writer.Write(option.GetType().Name);

            using var ms = new MemoryStream();
            using var tempWriter = new BinaryWriter(ms);
            option.WriteValue(tempWriter);
            var bytes = ms.ToArray();

            writer.Write(bytes.Length);
            writer.Write(bytes);
        }
    }

    private void LoadOptionValuesFromFile()
    {
        if (!File.Exists(FilePath)) return;
        using var file = File.OpenRead(FilePath);
        using var reader = new BinaryReader(file);

        var optionsCount = reader.ReadInt32();
        for (var i = 0; i < optionsCount; i++)
        {
            var key = reader.ReadString();
            var typeName = reader.ReadString();
            var length = reader.ReadInt32();

            var positionBefore = file.Position;

            if (!Options.TryGetValue(key, out var option))
            {
                Ls.LogWarning($"[OPTIONS] Unknown option '{key}' - skipped");
                file.Position = positionBefore + length;
                continue;
            }

            var expectedTypeName = option.GetType().Name;
            if (expectedTypeName != typeName)
            {
                Ls.LogWarning($"[OPTIONS] Mismatched type for '{key}': expected {expectedTypeName}, found {typeName} - skipped");
                file.Position = positionBefore + length;
                continue;
            }

            try
            {
                option.ReadValue(reader);
            }
            catch (Exception e)
            {
                Ls.LogWarning($"[OPTIONS] Failed to read option '{key}': {e.Message} - skipped");
                file.Position = positionBefore + length;
            }
        }
    }

    private void InitOptionProperties()
    {
        var type = GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var key = property.Name;
            var title = key;

            var optionName = property.GetCustomAttribute<OptionNameAttribute>();
            if (optionName != null)
            {
                title = optionName.Title;
                key = optionName.Key ?? key;
            }

            var textAttribute = property.GetCustomAttribute<TextOptionAttribute>();
            if (textAttribute != null)
            {
                RegisterTextOption(property, textAttribute, key, title);
                continue;
            }

            var boolAttribute = property.GetCustomAttribute<BoolOptionAttribute>();
            if (boolAttribute != null)
            {
                RegisterBoolOption(property, boolAttribute, key, title);
                continue;
            }

            var colorAttribute = property.GetCustomAttribute<ColorOptionAttribute>();
            if (colorAttribute != null)
            {
                RegisterColorOption(property, colorAttribute, key, title);
                continue;
            }

            var numberAttribute = property.GetCustomAttribute<NumberOptionAttribute>();
            if (numberAttribute != null)
            {
                RegisterNumberOption(property, numberAttribute, key, title);
                continue;
            }

            var enumAttribute = property.GetCustomAttribute<EnumOptionAttribute>();
            if (enumAttribute != null)
            {
                RegisterEnumOption(property, enumAttribute, key, title);
            }
        }
    }

    private void RegisterTextOption(
        PropertyInfo property,
        TextOptionAttribute attribute,
        string key,
        string title
    )
    {
        TextSerializableOption option;
        if (property.PropertyType == typeof(TextLocalOption))
        {
            var local = new TextLocalOption(key, title, attribute.DefaultValue);
            property.SetValue(this, local);
            option = local;
        }
        else if (property.PropertyType == typeof(TextSerializableOption))
        {
            option = new TextSerializableOption(key, title, attribute.DefaultValue);
            property.SetValue(this, option);
        }
        else
        {
            throw new Exception($"[Options] Unsupported text option property type: {property.PropertyType.FullName}");
        }

        RegisterPropertyOption(property, option);
    }

    private void RegisterBoolOption(
        PropertyInfo property,
        BoolOptionAttribute attribute,
        string key,
        string title
    )
    {
        BoolSerializableOption option;
        if (property.PropertyType == typeof(BoolLocalOption))
        {
            var local = new BoolLocalOption(key, title, attribute.DefaultValue);
            property.SetValue(this, local);
            option = local;
        }
        else if (property.PropertyType == typeof(BoolSerializableOption))
        {
            option = new BoolSerializableOption(key, title, attribute.DefaultValue);
            property.SetValue(this, option);
        }
        else
        {
            throw new Exception($"[Options] Unsupported bool option property type: {property.PropertyType.FullName}");
        }

        RegisterPropertyOption(property, option);
    }

    private void RegisterColorOption(
        PropertyInfo property,
        ColorOptionAttribute attribute,
        string key,
        string title
    )
    {
        ColorSerializableOption option;
        if (property.PropertyType == typeof(ColorLocalOption))
        {
            var local = new ColorLocalOption(key, title, attribute.DefaultValue);
            property.SetValue(this, local);
            option = local;
        }
        else if (property.PropertyType == typeof(ColorSerializableOption))
        {
            option = new ColorSerializableOption(key, title, attribute.DefaultValue);
            property.SetValue(this, option);
        }
        else
        {
            throw new Exception($"[Options] Unsupported color option property type: {property.PropertyType.FullName}");
        }

        RegisterPropertyOption(property, option);
    }

    private void RegisterNumberOption(
        PropertyInfo property,
        NumberOptionAttribute attribute,
        string key,
        string title
    )
    {
        NumberSerializableOption option;
        if (property.PropertyType == typeof(NumberLocalOption))
        {
            var local = new NumberLocalOption(
                key,
                title,
                attribute.DefaultValue,
                attribute.IncrementValue,
                attribute.MinValue,
                attribute.MaxValue,
                attribute.ValuePrefix,
                attribute.ValueSuffix
            );
            property.SetValue(this, local);
            option = local;
        }
        else if (property.PropertyType == typeof(NumberSerializableOption))
        {
            option = new NumberSerializableOption(
                key,
                title,
                attribute.DefaultValue,
                attribute.IncrementValue,
                attribute.MinValue,
                attribute.MaxValue,
                attribute.ValuePrefix,
                attribute.ValueSuffix
            );
            property.SetValue(this, option);
        }
        else
        {
            throw new Exception($"[Options] Unsupported number option property type: {property.PropertyType.FullName}");
        }

        RegisterPropertyOption(property, option);
    }

    private void RegisterEnumOption(
        PropertyInfo property,
        EnumOptionAttribute attribute,
        string key,
        string title
    )
    {
        EnumSerializableOption option;
        if (property.PropertyType == typeof(EnumLocalOption))
        {
            var local = new EnumLocalOption(key, title, attribute.DefaultValue);
            property.SetValue(this, local);
            option = local;
        }
        else if (property.PropertyType == typeof(EnumSerializableOption))
        {
            option = new EnumSerializableOption(key, title, attribute.DefaultValue);
            property.SetValue(this, option);
        }
        else
        {
            throw new Exception($"[Options] Unsupported enum option property type: {property.PropertyType.FullName}");
        }
        
        RegisterPropertyOption(property, option);
    }

    protected virtual void RegisterPropertyOption(PropertyInfo property, AbstractSerializableOption option)
    {
        RegisterOption(option);
        var lockedUnderHashAttribute = property.GetCustomAttribute<LockedUnderHashAttribute>();
        if (lockedUnderHashAttribute != null)
        {
            FeatureCodeBehaviour.Instance?.RegisterHash(lockedUnderHashAttribute.Hash);
            option.SetIsLockedFunc(() => !FeatureCodeBehaviour.Instance?.IsUnlocked(lockedUnderHashAttribute.Hash) ?? false);
        }

        var hiddenUnderHashAttribute = property.GetCustomAttribute<HiddenUnderHashAttribute>();
        if (hiddenUnderHashAttribute != null)
        {
            FeatureCodeBehaviour.Instance?.RegisterHash(hiddenUnderHashAttribute.Hash);
            option.SetIsHiddenFunc(() => !FeatureCodeBehaviour.Instance?.IsUnlocked(hiddenUnderHashAttribute.Hash) ?? false);
        }
    }

    private void RegisterOption(AbstractSerializableOption option)
    {
        option.ValueChanged += ValueChangedHandler;
        Options.Add(option.Key, option);
    }

    private void UnregisterOption(AbstractSerializableOption option)
    {
        option.ValueChanged -= ValueChangedHandler;
        Options.Remove(option.Key);
    }
}