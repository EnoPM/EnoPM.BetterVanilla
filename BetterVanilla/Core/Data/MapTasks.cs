using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using BetterVanilla.Core.Extensions;

namespace BetterVanilla.Core.Data;

public sealed class MapTasks
{
    private static List<MapTasks> AllMapTasks { get; } = [];
    
    public static MapTasks TheSkeld { get; } = new(0)
    {
        CommonTasks =
        {
            TaskTypes.SwipeCard,
            TaskTypes.FixWiring
        },
        LongTasks =
        {
            TaskTypes.ClearAsteroids,
            TaskTypes.AlignEngineOutput,
            TaskTypes.SubmitScan,
            TaskTypes.InspectSample,
            TaskTypes.FuelEngines,
            TaskTypes.StartReactor,
            TaskTypes.EmptyChute,
            TaskTypes.EmptyGarbage
        },
        ShortTasks =
        {
            TaskTypes.UploadData,
            TaskTypes.CalibrateDistributor,
            TaskTypes.ChartCourse,
            TaskTypes.CleanO2Filter,
            TaskTypes.UnlockManifolds,
            TaskTypes.UploadData,
            TaskTypes.StabilizeSteering,
            TaskTypes.UploadData,
            TaskTypes.PrimeShields,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.VentCleaning
        }
    };

    public static MapTasks MiraHq { get; } = new(1)
    {
        CommonTasks =
        {
            TaskTypes.FixWiring,
            TaskTypes.EnterIdCode
        },
        LongTasks =
        {
            TaskTypes.SubmitScan,
            TaskTypes.ClearAsteroids,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.WaterPlants,
            TaskTypes.StartReactor,
            TaskTypes.DivertPower
        },
        ShortTasks =
        {
            TaskTypes.ChartCourse,
            TaskTypes.CleanO2Filter,
            TaskTypes.FuelEngines,
            TaskTypes.AssembleArtifact,
            TaskTypes.SortSamples,
            TaskTypes.PrimeShields,
            TaskTypes.EmptyGarbage,
            TaskTypes.MeasureWeather,
            TaskTypes.DivertPower,
            TaskTypes.BuyBeverage,
            TaskTypes.ProcessData,
            TaskTypes.RunDiagnostics,
            TaskTypes.UnlockManifolds,
            TaskTypes.VentCleaning
        }
    };

    public static MapTasks Polus { get; } = new(2)
    {
        CommonTasks =
        {
            TaskTypes.SwipeCard,
            TaskTypes.InsertKeys,
            TaskTypes.ScanBoardingPass,
            TaskTypes.FixWiring
        },
        LongTasks =
        {
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.StartReactor,
            TaskTypes.FuelEngines,
            TaskTypes.OpenWaterways,
            TaskTypes.InspectSample,
            TaskTypes.ReplaceWaterJug,
            TaskTypes.FixWeatherNode,
            TaskTypes.FixWeatherNode,
            TaskTypes.FixWeatherNode,
            TaskTypes.FixWeatherNode,
            TaskTypes.RebootWifi
        },
        ShortTasks =
        {
            TaskTypes.MonitorOxygen,
            TaskTypes.UnlockManifolds,
            TaskTypes.StoreArtifacts,
            TaskTypes.FillCanisters,
            TaskTypes.EmptyGarbage,
            TaskTypes.ChartCourse,
            TaskTypes.SubmitScan,
            TaskTypes.ClearAsteroids,
            TaskTypes.FixWeatherNode,
            TaskTypes.FixWeatherNode,
            TaskTypes.AlignTelescope,
            TaskTypes.RepairDrill,
            TaskTypes.RecordTemperature,
            TaskTypes.RecordTemperature
        }
    };

    public static MapTasks Airship { get; } = new(4)
    {
        CommonTasks =
        {
            TaskTypes.FixWiring,
            TaskTypes.EnterIdCode
        },
        LongTasks =
        {
            TaskTypes.CalibrateDistributor,
            TaskTypes.ResetBreakers,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UnlockSafe,
            TaskTypes.StartFans,
            TaskTypes.EmptyGarbage,
            TaskTypes.EmptyGarbage,
            TaskTypes.EmptyGarbage,
            TaskTypes.DevelopPhotos,
            TaskTypes.FuelEngines,
            TaskTypes.RewindTapes,
            TaskTypes.EmptyGarbage,
            TaskTypes.EmptyGarbage
        },
        ShortTasks =
        {
            TaskTypes.PolishRuby,
            TaskTypes.StabilizeSteering,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.UploadData,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.DivertPower,
            TaskTypes.PickUpTowels,
            TaskTypes.CleanToilet,
            TaskTypes.DressMannequin,
            TaskTypes.SortRecords,
            TaskTypes.PutAwayPistols,
            TaskTypes.PutAwayRifles,
            TaskTypes.Decontaminate,
            TaskTypes.MakeBurger,
            TaskTypes.FixShower,
            TaskTypes.VentCleaning
        }
    };

    public static MapTasks TheFungle { get; } = new(5)
    {
        CommonTasks =
        {
            TaskTypes.CollectSamples,
            TaskTypes.EnterIdCode,
            TaskTypes.ReplaceParts,
            TaskTypes.RoastMarshmallow
        },
        LongTasks =
        {
            TaskTypes.CatchFish,
            TaskTypes.CollectVegetables,
            TaskTypes.ExtractFuel,
            TaskTypes.HelpCritter,
            TaskTypes.HoistSupplies,
            TaskTypes.PolishGem,
            TaskTypes.MineOres,
            TaskTypes.ReplaceWaterJug,
            TaskTypes.WaterPlants
        },
        ShortTasks =
        {
            TaskTypes.AssembleArtifact,
            TaskTypes.BuildSandcastle,
            TaskTypes.CollectShells,
            TaskTypes.CrankGenerator,
            TaskTypes.EmptyGarbage,
            TaskTypes.EmptyGarbage,
            TaskTypes.FixAntenna,
            TaskTypes.FixWiring,
            TaskTypes.LiftWeights,
            TaskTypes.MonitorMushroom,
            TaskTypes.PlayVideogame,
            TaskTypes.RecordTemperature,
            TaskTypes.RecordTemperature,
            TaskTypes.RecordTemperature,
            TaskTypes.TestFrisbee,
            TaskTypes.TuneRadio
        }
    };

    public static MapTasks? Current
    {
        get
        {
            if (GameOptionsManager.Instance == null) return null;
            var options = GameOptionsManager.Instance.CurrentGameOptions;
            if (options == null) return null;
            var result = AllMapTasks.FirstOrDefault(x => x.MapId == options.MapId);
            if (result == null)
            {
                Ls.LogMessage($"No map tasks found for {options.MapId}");
            }
            return result;
        }
    }

    private byte MapId { get; }

    private List<TaskTypes> CommonTasks { get; } = [];
    private List<TaskTypes> LongTasks { get; } = [];
    private List<TaskTypes> ShortTasks { get; } = [];

    private MapTasks(byte mapId)
    {
        MapId = mapId;
        AllMapTasks.Add(this);
    }

    private void ApplyTaskMoving()
    {
        TaskMover.Refresh();
        TaskMover.Apply(CommonTasks, LongTasks, ShortTasks);
    }
    
    public void RefreshOptions()
    {
        if (GameSettingMenu.Instance == null
            || GameSettingMenu.Instance.GameSettingsTab == null
            || GameSettingMenu.Instance.GameSettingsTab.Children == null) return;
        ApplyTaskMoving();
        foreach (var optionBehaviour in GameSettingMenu.Instance.GameSettingsTab.Children)
        {
            if (optionBehaviour == null || optionBehaviour.Data == null) continue;
            if (optionBehaviour.Data.Type != OptionTypes.Int) continue;
            var numberOption = optionBehaviour.As<NumberOption>();
            if (numberOption == null) continue;
            if (numberOption.intOptionName == Int32OptionNames.NumCommonTasks)
            {
                RefreshOptionBehaviour(numberOption, CommonTasks.ToHashSet().Count);
            }
            else if (numberOption.intOptionName == Int32OptionNames.NumLongTasks)
            {
                RefreshOptionBehaviour(numberOption, LongTasks.ToHashSet().Count);
            }
            else if (numberOption.intOptionName == Int32OptionNames.NumShortTasks)
            {
                RefreshOptionBehaviour(numberOption, ShortTasks.ToHashSet().Count);
            }
        }
    }

    private static void RefreshOptionBehaviour(NumberOption numberOption, int max)
    {
        if (!numberOption.Is<IntGameSetting>(out var data))
        {
            return;
        }
        data.ValidRange = new IntRange(0, max);
        numberOption.ValidRange = new FloatRange(0, max);
        if (numberOption.GetInt() > max)
        {
            numberOption.Value = max;
            numberOption.UpdateValue();
            numberOption.OnValueChanged?.Invoke(numberOption);
        }
        numberOption.AdjustButtonsActiveState();
    }
}