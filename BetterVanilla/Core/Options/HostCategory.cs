using System.Collections.Generic;

namespace BetterVanilla.Core.Options;

public sealed class HostCategory : BaseCategory
{
    public static readonly List<HostCategory> AllCategories = [];
    
    public readonly RulesCategory GameOptionsMenuCategory;
    public readonly List<BaseHostOption> AllOptions = [];

    public HostCategory(string name) : base(name)
    {
        GameOptionsMenuCategory = new RulesCategory
        {
            CategoryName = StringNames.None,
            AllGameSettings = new Il2CppSystem.Collections.Generic.List<BaseGameSetting>()
        };
        
        AllCategories.Add(this);
    }

    private void RegisterInCategory(BaseHostOption option)
    {
        AllOptions.Add(option);
        GameOptionsMenuCategory.AllGameSettings.Add(option.GameSetting);
    }

    public BoolHostOption CreateBool(string name, string title, bool defaultValue)
    {
        var option = new BoolHostOption(name, title, defaultValue);
        RegisterInCategory(option);
        return option;
    }

    public IntHostOption CreateInt(string name, string title, int defaultValue, int increment, IntRange validRange, string formatString, bool zeroIsInfinity, NumberSuffixes suffixType)
    {
        var option = new IntHostOption(name, title, defaultValue, increment, validRange, formatString, zeroIsInfinity, suffixType);
        RegisterInCategory(option);
        return option;
    }
    
    public FloatHostOption CreateFloat(string name, string title, float defaultValue, float increment, FloatRange validRange, string formatString, bool zeroIsInfinity, NumberSuffixes suffixType)
    {
        var option = new FloatHostOption(name, title, defaultValue, increment, validRange, formatString, zeroIsInfinity, suffixType);
        RegisterInCategory(option);
        return option;
    }
}