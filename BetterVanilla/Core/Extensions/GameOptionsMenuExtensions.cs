using AmongUs.GameOptions;
using BetterVanilla.Options;
using BetterVanilla.Options.Core.Host;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class GameOptionsMenuExtensions
{
    public static bool CustomValueChanged(this GameOptionsMenu menu, OptionBehaviour optionBehaviour)
    {
        var customOption = HostOptions.Default.FindOptionByBehaviour(optionBehaviour);
        if (customOption == null) return true;
        customOption.UpdateValueFromSettingBehaviour();
        return false;
    }

    public static void CreateBetterSettings(this GameOptionsMenu menu)
    {
        var y = 0.713f;
        foreach (var allCategory in GameManager.Instance.GetAllCategories())
        {
            var isCustomCategory = HostOptions.Default.MenuCategory.Pointer == allCategory.Pointer;
            var categoryHeaderMasked = Object.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
            if (isCustomCategory)
            {
                categoryHeaderMasked.CustomSetHeader(20);
            }
            else
            {
                categoryHeaderMasked.SetHeader(allCategory.CategoryName, 20);
            }
            categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
            categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, y, -2f);
            y -= 0.63f;
            foreach (var gameSetting in allCategory.AllGameSettings)
            {
                var customHostOption = isCustomCategory ? HostOptions.Default.FindOptionByGameSetting(gameSetting) : null;
                UpdateSettingConstraints(gameSetting);
                switch (gameSetting.Type)
                {
                    case OptionTypes.Checkbox:
                        var toggleOptionBehaviour = Object.Instantiate(menu.checkboxOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                        toggleOptionBehaviour.transform.localPosition = new Vector3(0.952f, y, -2f);
                        toggleOptionBehaviour.SetClickMask(menu.ButtonClickMask);
                        toggleOptionBehaviour.SetUpFromData(gameSetting, 20);
                        if (customHostOption is BoolHostOption customBoolOption)
                        {
                            customBoolOption.OnBehaviourCreated(toggleOptionBehaviour);
                        }
                        menu.Children.Add(toggleOptionBehaviour);
                        break;
                    case OptionTypes.String:
                        var stringOptionBehaviour = Object.Instantiate(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                        stringOptionBehaviour.transform.localPosition = new Vector3(0.952f, y, -2f);
                        stringOptionBehaviour.SetClickMask(menu.ButtonClickMask);
                        stringOptionBehaviour.SetUpFromData(gameSetting, 20);
                        menu.Children.Add(stringOptionBehaviour);
                        break;
                    case OptionTypes.Float:
                    case OptionTypes.Int:
                        var numberOptionBehaviour = Object.Instantiate(menu.numberOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                        numberOptionBehaviour.transform.localPosition = new Vector3(0.952f, y, -2f);
                        numberOptionBehaviour.SetClickMask(menu.ButtonClickMask);
                        numberOptionBehaviour.SetUpFromData(gameSetting, 20);
                        if (customHostOption is NumberHostOption customNumberOption)
                        {
                            customNumberOption.OnBehaviourCreated(numberOptionBehaviour);
                        }
                        menu.Children.Add(numberOptionBehaviour);
                        break;
                    case OptionTypes.Player:
                        var playerOptionBehaviour = Object.Instantiate(menu.playerOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                        playerOptionBehaviour.transform.localPosition = new Vector3(0.952f, y, -2f);
                        playerOptionBehaviour.SetClickMask(menu.ButtonClickMask);
                        playerOptionBehaviour.SetUpFromData(gameSetting, 20);
                        menu.Children.Add(playerOptionBehaviour);
                        break;
                }
                y -= 0.45f;
            }
        }
        var children = menu.scrollBar.GetComponentsInChildren<UiElement>();
        foreach (var child in children)
        {
            menu.ControllerSelectable.Add(child);
        }
        menu.scrollBar.SetYBoundsMax((float)(-(double)y - 1.649999976158142));
    }

    private static void UpdateSettingConstraints(BaseGameSetting gameSetting)
    {
        switch (gameSetting.Type)
        {
            case OptionTypes.Float:
                UpdateFloatSettingConstraints(gameSetting.As<FloatGameSetting>());
                break;
            case OptionTypes.Int:
                UpdateIntSettingConstraints(gameSetting.As<IntGameSetting>());
                break;
        }
    }

    private static void UpdateFloatSettingConstraints(FloatGameSetting gameSetting)
    {
        switch (gameSetting.OptionName)
        {
            case FloatOptionNames.KillCooldown:
                gameSetting.ValidRange = new FloatRange(10f, 60f);
                gameSetting.Increment = 0.5f;
                break;
            case FloatOptionNames.ImpostorLightMod:
            case FloatOptionNames.CrewLightMod:
            case FloatOptionNames.PlayerSpeedMod:
                gameSetting.ValidRange = new FloatRange(0.1f, 3f);
                gameSetting.Increment = 0.05f;
                break;
        }
    }

    private static void UpdateIntSettingConstraints(IntGameSetting gameSetting)
    {
        switch (gameSetting.OptionName)
        {
            case Int32OptionNames.EmergencyCooldown:
                gameSetting.ValidRange = new IntRange(0, 120);
                gameSetting.Increment = 1;
                break;
            case Int32OptionNames.DiscussionTime:
            case Int32OptionNames.VotingTime:
                gameSetting.Increment = 1;
                break;
            case Int32OptionNames.NumCommonTasks:
                gameSetting.ValidRange = new IntRange(0, 4);
                break;
            case Int32OptionNames.NumLongTasks:
                gameSetting.ValidRange = new IntRange(0, 6);
                break;
            case Int32OptionNames.NumShortTasks:
                gameSetting.ValidRange = new IntRange(0, 23);
                break;
        }
    }
}