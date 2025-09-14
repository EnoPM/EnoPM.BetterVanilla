using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
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
    [OptionName("Allow team preferences")]
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
    
    [BoolOption(false)]
    [OptionName("Common tasks as non-common")]
    public BoolHostOption DefineCommonTasksAsNonCommon { get; set; } = null!;

    private HostOptions() : base("host")
    {
        MenuCategory = new RulesCategory
        {
            CategoryName = StringNames.None,
            AllGameSettings = new Il2CppSystem.Collections.Generic.List<BaseGameSetting>()
        };
        
        PolusReactorCountdown.SetIsLockedFunc(IsNotPolusMap);
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

        DefineCommonTasksAsNonCommon.ValueChanged += OnDefineCommonTasksAsNonCommonValueChanged;
    }

    private static void OnDefineCommonTasksAsNonCommonValueChanged()
    {
        MapTasks.Current?.RefreshOptions();
    }

    private bool IsProtectionDisabled()
    {
        return !ProtectFirstKilledPlayer.Value;
    }

    private static bool IsNotPolusMap()
    {
        return GameOptionsManager.Instance.CurrentGameOptions.MapId != (byte)MapNames.Polus;
    }
    
    public void ShareAllOptions()
    {
        if (!LocalConditions.AmHost() || BetterPlayerControl.LocalPlayer == null) return;
        foreach (var option in GetOptions())
        {
            BetterPlayerControl.LocalPlayer.RpcSetHostOptionValue(option);
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