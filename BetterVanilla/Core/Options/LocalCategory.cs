using System;
using System.Collections.Generic;
using System.Reflection;
using BetterVanilla.Core.Attributes;

namespace BetterVanilla.Core.Options;

public sealed class LocalCategory : BaseCategory
{
    public static readonly List<LocalCategory> AllCategories = [];
    
    public readonly List<BaseLocalOption> AllOptions = [];

    public LocalCategory(string name) : base(name)
    {
        AllCategories.Add(this);
    }

    private void RegisterInCategory(BaseLocalOption option)
    {
        AllOptions.Add(option);
    }

    public BoolLocalOption CreateBool(string name, string title, bool defaultValue)
    {
        var option = new BoolLocalOption(name, title, defaultValue);
        RegisterInCategory(option);
        return option;
    }

    public IntLocalOption CreateInt(string name, string title, int defaultValue, int increment, IntRange validRange, string prefix = "", string suffix = "")
    {
        var option = new IntLocalOption(name, title, defaultValue, increment, validRange, prefix, suffix);
        RegisterInCategory(option);
        return option;
    }
    
    public FloatLocalOption CreateFloat(string name, string title, float defaultValue, float increment, FloatRange validRange, string prefix = "", string suffix = "")
    {
        var option = new FloatLocalOption(name, title, defaultValue, increment, validRange, prefix, suffix);
        RegisterInCategory(option);
        return option;
    }

    public StringLocalOption CreateEnum<TEnum>(string name, string title, TEnum defaultValue) where TEnum : struct
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enum", enumType.FullName);
        }

        var enumValues = Enum.GetValues(enumType);
        var values = new List<string>();
        var realValues = new List<string>();
        var defaultStringValue = defaultValue.ToString();
        foreach (var enumValue in enumValues)
        {
            var key = (TEnum)enumValue;
            var fieldName = key.ToString();
            if (fieldName == null)
            {
                throw new Exception($"Unable to stringify enum key {key} in {enumType.FullName}");
            }
            var field = enumType.GetField(fieldName);
            if (field == null)
            {
                throw new Exception($"Unable to find field {fieldName} in {enumType.FullName}");
            }
            var attribute = field.GetCustomAttribute<NamedFieldAttribute>();
            var prettyName = attribute == null ? fieldName : attribute.Name;
            values.Add(prettyName);
            realValues.Add(fieldName);
            if (fieldName == defaultStringValue)
            {
                defaultStringValue = prettyName;
            }
        }
        var option = new StringLocalOption(name, title, defaultStringValue, values, realValues);
        RegisterInCategory(option);
        return option;
    }
}