using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core;
using BetterVanilla.Options.Core.Host;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options;

public class HostOptions : AbstractSerializableOptionHolder
{
    public static readonly HostOptions Default = new();

    public readonly RulesCategory MenuCategory;

    [BoolOption(true)]
    [OptionName("Allow realtime vote display")]
    public BoolHostOption AllowDeadVoteDisplay { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Allow team Preferences")]
    public BoolHostOption AllowTeamPreference { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Hide dead player pets")]
    public BoolHostOption HideDeadPlayerPets { get; set; } = null!;
    
    [NumberOption(60f, 15f, 120f, IncrementValue = 0.5f, ValueSuffix = "s")]
    [OptionName("Polus reactor countdown")]
    public NumberHostOption PolusReactorCountdown { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Protect first killed player")]
    public BoolHostOption ProtectFirstKilledPlayer { get; set; } = null!;
    
    [NumberOption(60f, 15f, 300f, IncrementValue = 1f, ValueSuffix = "s")]
    [OptionName("Protection duration")]
    public NumberHostOption ProtectionDuration { get; set; } = null!;

    public HostOptions() : base("host")
    {
        MenuCategory = new RulesCategory
        {
            CategoryName = StringNames.None,
            AllGameSettings = new Il2CppSystem.Collections.Generic.List<BaseGameSetting>()
        };
        
        PolusReactorCountdown.SetIsLockedFunc(IsNotPolusMap);
        HideDeadPlayerPets.SetIsLockedFunc(IsNotPolusMap);
        ProtectionDuration.SetIsLockedFunc(IsProtectionDisabled);

        foreach (var option in GetOptions())
        {
            if (option is BoolHostOption boolOption)
            {
                MenuCategory.AllGameSettings.Add(boolOption.GameSetting);
            }
            else if (option is NumberHostOption checkboxOption)
            {
                MenuCategory.AllGameSettings.Add(checkboxOption.GameSetting);
            }
            else
            {
                Ls.LogWarning($"Unsupported host option {option.Key}");
            }
        }
    }

    private bool IsProtectionDisabled()
    {
        return !ProtectFirstKilledPlayer.Value;
    }

    private bool IsNotPolusMap()
    {
        return GameOptionsManager.Instance.CurrentGameOptions.MapId != (byte)MapNames.Polus;
    }
    
    public void ShareAllOptions()
    {
        if (!LocalConditions.AmHost()) return;
        foreach (var option in GetOptions())
        {
            PlayerControl.LocalPlayer.RpcShareHostOption(option);
        }
    }

    public IBaseHostOption? FindOptionByBehaviour(OptionBehaviour behaviour)
    {
        foreach (var option in GetOptions())
        {
            if (option is not IBaseHostOption hostOption) continue;
            if (hostOption.GetOptionBehaviour() == behaviour)
            {
                return hostOption;
            }
        }
        return null;
    }
    
    public IBaseHostOption? FindOptionByGameSetting(BaseGameSetting gameSetting)
    {
        foreach (var option in GetOptions())
        {
            if (option is not IBaseHostOption hostOption) continue;
            if (hostOption.GetGameSetting() == gameSetting)
            {
                return hostOption;
            }
        }
        return null;
    }
}