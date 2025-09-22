using System.Collections.Generic;
using BetterVanilla.Options;

namespace BetterVanilla.Core.Extensions;

public static class GameManagerExtensions
{
    public static List<RulesCategory> GetAllCategories(this GameManager gameManager)
    {
        var results = new List<RulesCategory>
        {
            HostOptions.Default.MenuCategory
        };

        foreach (var category in gameManager.GameSettingsList.AllCategories)
        {
            results.Add(category);
        }

        return results;
    }
}