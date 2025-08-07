using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Options.Components;
using BetterVanilla.Options.Components.Controllers;
using BetterVanilla.Options.Core.Local;
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

    protected virtual void Update()
    {
        foreach (var option in AllOptions)
        {
            option.RefreshVisibility();
            option.SetActive(!option.IsHidden && option.IsMatchingFilter);
        }
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
            option.IsMatchingFilter = option.MatchSearch(filterText);
        }
    }

    protected void InitSerializableOptions(AbstractSerializableOptionHolder holder)
    {
        var options = holder.GetOptions();
        foreach (var option in options)
        {
            if (option is BoolLocalOption boolOption)
            {
                var ui = Instantiate(toggleOptionPrefab, tabContent);
                ui.SetOption(boolOption);
                AllOptions.Add(ui);
            }
            else if (option is ColorLocalOption colorOption)
            {
                var ui = Instantiate(colorOptionPrefab, tabContent);
                ui.SetOption(colorOption);
                AllOptions.Add(ui);
            }
            else if (option is NumberLocalOption numberOption)
            {
                var ui = Instantiate(numberOptionPrefab, tabContent);
                ui.SetOption(numberOption);
                AllOptions.Add(ui);
            }
            else if (option is TextLocalOption textOption)
            {
                var ui = Instantiate(textOptionPrefab, tabContent);
                ui.SetOption(textOption);
                AllOptions.Add(ui);
            }
            else if (option is EnumLocalOption enumOption)
            {
                var ui = Instantiate(enumOptionPrefab, tabContent);
                ui.SetOption(enumOption);
                AllOptions.Add(ui);
            }
        }
    }
}