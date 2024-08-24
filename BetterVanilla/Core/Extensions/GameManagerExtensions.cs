using System.Collections.Generic;
using BetterVanilla.Core.Options;

namespace BetterVanilla.Core.Extensions;

public static class GameManagerExtensions
{
    public static List<RulesCategory> GetAllCategories(this GameManager gameManager)
    {
        var results = new List<RulesCategory>();

        foreach (var category in HostCategory.AllCategories)
        {
            results.Add(category.GameOptionsMenuCategory);
        }

        foreach (var category in gameManager.GameSettingsList.AllCategories)
        {
            results.Add(category);
        }

        return results;
    }
}