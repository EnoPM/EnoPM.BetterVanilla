using System.Linq;
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
    
    [BoolOption(false)]
    [OptionName("Better Polus")]
    public BoolHostOption BetterPolus { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Randomize fix wiring task order")]
    public BoolHostOption RandomizeFixWiringTaskOrder { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Randomize upload task location")]
    public BoolHostOption RandomizeUploadTaskLocation { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Randomize player order in meetings")]
    public BoolHostOption RandomizePlayerOrderInMeetings { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Anonymize players during cameras when lights are off")]
    public BoolHostOption AnonymizePlayersOnCamerasDuringLights { get; set; } = null!;

    private HostOptions() : base("host")
    {
        MenuCategory = new RulesCategory
        {
            CategoryName = StringNames.None,
            AllGameSettings = new Il2CppSystem.Collections.Generic.List<BaseGameSetting>()
        };
        
        PolusReactorCountdown.SetIsLockedFunc(IsNotPolusMap);
        ProtectionDuration.SetIsLockedFunc(IsProtectionDisabled);
        BetterPolus.SetIsLockedFunc(IsBetterPolusNotAllowed);
        RandomizeFixWiringTaskOrder.SetIsLockedFunc(IsNotAllBetterVanillaPlayers);
        RandomizeUploadTaskLocation.SetIsLockedFunc(IsNotAllBetterVanillaPlayers);
        RandomizePlayerOrderInMeetings.SetIsLockedFunc(IsNotAllBetterVanillaPlayers);
        AnonymizePlayersOnCamerasDuringLights.SetIsLockedFunc(IsNotAllBetterVanillaPlayers);

        foreach (var option in GetOptions())
        {
            switch (option)
            {
                case BoolHostOption boolOption:
                    MenuCategory.AllGameSettings.Add(boolOption.GameSetting);
                    break;
                case NumberHostOption checkboxOption:
                    MenuCategory.AllGameSettings.Add(checkboxOption.GameSetting);
                    break;
                default:
                    Ls.LogWarning($"Unsupported host option type {option.GetType().Name} for '{option.Key}'");
                    break;
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
    
    private static bool IsBetterPolusNotAllowed() => IsNotPolusMap() || IsNotAllBetterVanillaPlayers();

    private static bool IsNotAllBetterVanillaPlayers() => !LocalConditions.IsAllPlayersUsingBetterVanilla();

    private static bool IsNotPolusMap()
    {
        return GameOptionsManager.Instance.CurrentGameOptions.MapId != (byte)MapNames.Polus;
    }
    
    public void ShareAllOptions()
    {
        if (!LocalConditions.AmHost() || BetterPlayerControl.LocalPlayer == null) return;
        BetterPlayerControl.LocalPlayer.RpcShareAllHostOptions();
    }

    public IBaseHostOption? FindOptionByBehaviour(OptionBehaviour behaviour)
    {
        return GetOptions<IBaseHostOption>().FirstOrDefault(x => x.GetOptionBehaviour() == behaviour);
    }
    
    public IBaseHostOption? FindOptionByGameSetting(BaseGameSetting gameSetting)
    {
        return GetOptions<IBaseHostOption>().FirstOrDefault(x => x.GetGameSetting() == gameSetting);
    }
}