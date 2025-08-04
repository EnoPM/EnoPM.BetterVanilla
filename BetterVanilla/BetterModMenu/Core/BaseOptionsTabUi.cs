using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Options;
using BetterVanilla.Options.Components;
using BetterVanilla.Options.Components.Controllers;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.BetterModMenu.Core;

public abstract class BaseOptionsTabUi : BaseMenuTabUi
{
    public ToggleOptionUi toggleOptionPrefab = null!;
    public TextOptionUi textOptionPrefab = null!;
    public NumberOptionUi numberOptionPrefab = null!;
    public EnumOptionUi enumOptionPrefab = null!;
    public ColorOptionUi colorOptionPrefab = null!;
    
    public OptionFiltersUi filters = null!;
    
    public List<BaseOptionUi> AllOptions { get; } = [];

    public void OnFilterTextValueChanged(string _)
    {
        ApplyOptionFilters();
    }

    protected void ApplyOptionFilters()
    {
        this.StartCoroutine(CoApplyOptionFilters());
    }

    private IEnumerator CoApplyOptionFilters()
    {
        while (!filters || !filters.searchField)
        {
            yield return new WaitForEndOfFrame();
        }
        var filterText = filters.searchField.text.ToLowerInvariant();

        foreach (var option in AllOptions)
        {
            option.SetActive(option.MatchSearch(filterText));
        }
    }
    
    protected void InitSerializableOptions(AbstractSerializableOptionHolder holder)
    {
        var options = holder.GetOptions();
        foreach (var option in options)
        {
            if (option is BoolSerializableOption boolOption)
            {
                var ui = Instantiate(toggleOptionPrefab, tabContent);
                ui.SetOption(boolOption);
                AllOptions.Add(ui);
            }
            else if (option is ColorSerializableOption colorOption)
            {
                var ui = Instantiate(colorOptionPrefab, tabContent);
                ui.SetOption(colorOption);
                AllOptions.Add(ui);
            }
            else if (option is NumberSerializableOption numberOption)
            {
                var ui = Instantiate(numberOptionPrefab, tabContent);
                ui.SetOption(numberOption);
                AllOptions.Add(ui);
            }
            else if (option is TextSerializableOption textOption)
            {
                var ui = Instantiate(textOptionPrefab, tabContent);
                ui.SetOption(textOption);
                AllOptions.Add(ui);
            }
            else if (option is EnumSerializableOption enumOption)
            {
                var ui = Instantiate(enumOptionPrefab, tabContent);
                ui.SetOption(enumOption);
                AllOptions.Add(ui);
            }
        }
    }
}