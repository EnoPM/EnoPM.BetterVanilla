using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Options.Core.Host;
using BetterVanilla.Options.Core.Local;

namespace BetterVanilla.Options.Core.Serialization;

public abstract partial class AbstractSerializableOptionHolder
{
    private string FilePath { get; }
    private Dictionary<string, AbstractSerializableOption> Options { get; } = [];
    private Debouncer Debounce { get; set; }

    protected int DebounceDelayInSeconds { get; set; } = 5;

    protected AbstractSerializableOptionHolder(string filename)
    {
        FilePath = Path.Combine(ModPaths.OptionsDirectory, filename);
        Debounce = new Debouncer(TimeSpan.FromSeconds(DebounceDelayInSeconds));
        Debounce.Debounced += Save;

        InitOptionProperties();

        try
        {
            LoadOptionValuesFromFile();
        }
        catch (Exception ex)
        {
            Ls.LogWarning($"Failed to load option values from {FilePath}: {ex.Message}");
        }
    }

    public AbstractSerializableOption[] GetOptions()
    {
        return Options.Select(x => x.Value).ToArray();
    }

    public T[] GetOptions<T>()
    {
        var results = new List<T>();
        foreach (var (_, option) in Options)
        {
            if (option is not T returnType)
            {
                continue;
            }
            results.Add(returnType);
        }
        return results.ToArray();
    }

    public byte[] ToBytes()
    {
        using var output = ToStream();
        return ByteCompressor.Compress(output.ToArray());
    }

    private MemoryStream ToStream()
    {
        var output = new MemoryStream();
        using var writer = new BinaryWriter(output);
        
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
        
        output.Position = 0;
        return output;
    }

    public void FromBytes(byte[] bytes)
    {
        using var stream = new MemoryStream(ByteCompressor.Decompress(bytes));
        FromStream(stream);
    }

    private void FromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var key = reader.ReadString();
            var typeName = reader.ReadString();
            var length = reader.ReadInt32();
            
            var positionBefore = reader.BaseStream.Position;

            if (!Options.TryGetValue(key, out var option))
            {
                Ls.LogWarning($"[OPTIONS] Unknown option {key} - skipped");
                reader.BaseStream.Position = positionBefore + length;
                continue;
            }
            
            var expectedTypeName = option.GetType().Name;
            if (expectedTypeName != typeName)
            {
                Ls.LogWarning($"[OPTIONS] Mismatched type for '{key}': expected {expectedTypeName}, found {typeName} - skipped");
                reader.BaseStream.Position = positionBefore + length;
                continue;
            }

            try
            {
                option.ReadValue(reader);
            }
            catch (Exception e)
            {
                Ls.LogWarning($"[OPTIONS] Failed to read option '{key}': {e.Message} - skipped");
                reader.BaseStream.Position = positionBefore + length;
            }
        }
    }

    public void Save() => SaveOptionValuesToFile();

    private void SaveOptionValuesToFile()
    {
        Ls.LogInfo($"Saving options to file: {FilePath}");
        File.WriteAllBytes(FilePath, ToBytes());
    }

    private void LoadOptionValuesFromFile()
    {
        if (!File.Exists(FilePath)) return;
        var bytes = File.ReadAllBytes(FilePath);
        FromBytes(bytes);
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
            var local = new TextLocalOption(key, title, attribute.DefaultValue, attribute.MaxLength);
            property.SetValue(this, local);
            option = local;
        }
        else if (property.PropertyType == typeof(TextSerializableOption))
        {
            option = new TextSerializableOption(key, title, attribute.DefaultValue, attribute.MaxLength);
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
        else if (property.PropertyType == typeof(BoolHostOption))
        {
            option = new BoolHostOption(key, title, attribute.DefaultValue);
            property.SetValue(this, option);
        }
        else if (property.PropertyType == typeof(BoolSerializableOption))
        {
            var host = new BoolSerializableOption(key, title, attribute.DefaultValue);
            property.SetValue(this, host);
            option = host;
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
        else if (property.PropertyType == typeof(NumberHostOption))
        {
            var host = new NumberHostOption(
                key,
                title,
                attribute.DefaultValue,
                attribute.IncrementValue,
                attribute.MinValue,
                attribute.MaxValue,
                attribute.ValuePrefix,
                attribute.ValueSuffix
            );
            property.SetValue(this, host);
            option = host;
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
        option.ValueChanged += Debounce.Trigger;
        Options.Add(option.Key, option);
    }

    private void UnregisterOption(AbstractSerializableOption option)
    {
        option.ValueChanged -= Debounce.Trigger;
        Options.Remove(option.Key);
    }
}