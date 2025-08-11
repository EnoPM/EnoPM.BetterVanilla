using System;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Options.Core.Serialization;

public partial class AbstractSerializableOptionHolder
{
    [AttributeUsage(AttributeTargets.Property)]
    protected class LockedUnderHashAttribute(string hash) : Attribute
    {
        public string Hash { get; } = hash;
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    protected class HiddenUnderHashAttribute(string hash) : Attribute
    {
        public string Hash { get; } = hash;
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class OptionNameAttribute : Attribute
    {
        public string? Key { get; }
        public string Title { get; }

        public OptionNameAttribute(string key, string title)
        {
            Key = key;
            Title = title;
        }

        public OptionNameAttribute(string title)
        {
            Title = title;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class EnumOptionAttribute : Attribute
    {
        public object DefaultValue { get; }

        public EnumOptionAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class TextOptionAttribute : Attribute
    {
        public string DefaultValue { get; }
        public int MaxLength { get; }

        public TextOptionAttribute(string defaultValue, int maxLength)
        {
            DefaultValue = defaultValue;
            MaxLength = maxLength;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class BoolOptionAttribute : Attribute
    {
        public bool DefaultValue { get; }

        public BoolOptionAttribute(bool defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class ColorOptionAttribute : Attribute
    {
        public Color DefaultValue { get; }

        public ColorOptionAttribute(float r, float g, float b, float a = 1f)
        {
            DefaultValue = new Color(r, g, b, a);
        }

        public ColorOptionAttribute(string hexColor)
        {
            DefaultValue = ColorUtils.FromHex(hexColor);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    protected sealed class NumberOptionAttribute : Attribute
    {
        public float DefaultValue { get; }
        public float MinValue { get; }
        public float MaxValue { get; }
        public float IncrementValue { get; set; } = 1f;
        public string ValuePrefix { get; set; } = "";
        public string ValueSuffix { get; set; } = "";

        public NumberOptionAttribute(
            float defaultValue,
            float min,
            float max
        )
        {
            DefaultValue = defaultValue;
            MinValue = min;
            MaxValue = max;
        }
    }
}