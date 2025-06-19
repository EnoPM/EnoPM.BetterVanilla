using System.Collections.Generic;
using BetterVanilla.Options.Components;
using BetterVanilla.Options.Components.Controllers;

namespace BetterVanilla.BetterModMenu.Core;

public abstract class BaseOptionsTabUi : BaseMenuTabUi
{
    public ToggleOptionUi toggleOptionPrefab = null!;
    public TextOptionUi textOptionPrefab = null!;
    public NumberOptionUi numberOptionPrefab = null!;
    public EnumOptionUi enumOptionPrefab = null!;
    public ColorOptionUi colorOptionPrefab = null!;
    
    public OptionFiltersUi filters = null!;
    
    protected List<BaseOptionUi> AllOptions { get; } = [];

    public void OnFilterTextValueChanged(string _)
    {
        ApplyOptionFilters();
    }

    protected void ApplyOptionFilters()
    {
        var filterText = filters.searchField.text.ToLowerInvariant();

        foreach (var option in AllOptions)
        {
            option.SetActive(option.MatchSearch(filterText));
        }
    }
}