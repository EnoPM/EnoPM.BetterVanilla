using BetterVanilla.Core.Options;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class LobbyViewSettingsPaneExtensions
{
    public static void DrawBetterNormalTab(this LobbyViewSettingsPane pane)
    {
        var y1 = 1.44f;
        foreach (var allCategory in GameManager.Instance.GetAllCategories())
        {
            var customCategory = HostCategory.AllCategories.Find(x => x.GameOptionsMenuCategory == allCategory);
            var categoryHeaderMasked = Object.Instantiate(pane.categoryHeaderOrigin, pane.settingsContainer);
            if (customCategory != null)
            {
                categoryHeaderMasked.CustomSetHeader(61, customCategory);
            }
            else
            {
                categoryHeaderMasked.SetHeader(allCategory.CategoryName, 61);
            }
            categoryHeaderMasked.transform.localScale = Vector3.one;
            categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, y1, -2f);
            pane.settingsInfo.Add(categoryHeaderMasked.gameObject);
            var y2 = y1 - 0.85f;
            for (var index = 0; index < allCategory.AllGameSettings.Count; ++index)
            {
                var gameSetting = allCategory.AllGameSettings._items[index];
                var customOption = customCategory?.AllOptions.Find(x => x.GameSetting == gameSetting);
                var settingsInfoPanel = Object.Instantiate(pane.infoPanelOrigin, pane.settingsContainer);
                settingsInfoPanel.transform.localScale = Vector3.one;
                float x;
                if (index % 2 == 0)
                {
                    x = -8.95f;
                    if (index > 0)
                        y2 -= 0.59f;
                }
                else
                    x = -3f;
                settingsInfoPanel.transform.localPosition = new Vector3(x, y2, -2f);
                var num = customOption == null ? GameOptionsManager.Instance.CurrentGameOptions.GetValue(gameSetting) : -1f;
                if (gameSetting.Type == OptionTypes.Checkbox)
                {
                    if (customOption != null)
                    {
                        settingsInfoPanel.CustomSetInfoCheckbox(61, customOption);
                    }
                    else
                    {
                        settingsInfoPanel.SetInfoCheckbox(gameSetting.Title, 61, num > 0.0);
                    }
                }
                else
                {
                    if (customOption != null)
                    {
                        settingsInfoPanel.CustomSetInfo(61, customOption);
                    }
                    else
                    {
                        settingsInfoPanel.SetInfo(gameSetting.Title, gameSetting.GetValueString(num), 61);
                    }
                }
                pane.settingsInfo.Add(settingsInfoPanel.gameObject);
                customOption?.OnViewBehaviourCreated(settingsInfoPanel);
            }
            y1 = y2 - 0.59f;
        }
        pane.scrollBar.CalculateAndSetYBounds(pane.settingsInfo.Count + 10, 2f, 6f, 0.59f);
    }
}