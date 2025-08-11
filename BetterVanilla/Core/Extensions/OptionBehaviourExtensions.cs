using BetterVanilla.Options;

namespace BetterVanilla.Core.Extensions;

public static class OptionBehaviourExtensions
{
    public static bool CustomUpdateValue(this OptionBehaviour optionBehaviour)
    {
        var customOption = HostOptions.Default.FindOptionByBehaviour(optionBehaviour);
        return customOption == null;
    }
}