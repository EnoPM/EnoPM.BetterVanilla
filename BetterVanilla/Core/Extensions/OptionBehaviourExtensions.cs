using BetterVanilla.Core.Options;

namespace BetterVanilla.Core.Extensions;

public static class OptionBehaviourExtensions
{
    public static bool CustomUpdateValue(this OptionBehaviour optionBehaviour)
    {
        var customOption = BaseHostOption.AllOptions.Find(x => x.Behaviour == optionBehaviour);
        return customOption == null;
    }
}