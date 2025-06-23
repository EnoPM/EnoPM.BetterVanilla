using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.BetterModMenu.Tabs;

public sealed class SponsorTabUi : BaseOptionsTabUi
{
    public BecomeSponsorUi becomeSponsor = null!;
    
    private void Start()
    {
        var toggle = Instantiate(toggleOptionPrefab, tabContent);
        toggle.SetLabel("Toggle option");
        toggle.lockOverlay.SetActive(true);
        AllOptions.Add(toggle);
        
        var text = Instantiate(textOptionPrefab, tabContent);
        text.SetLabel("Text option");
        text.lockOverlay.SetActive(true);
        AllOptions.Add(text);
        
        var number = Instantiate(numberOptionPrefab, tabContent);
        number.SetLabel("Number option");
        number.lockOverlay.SetActive(true);
        AllOptions.Add(number);
        
        var enumOption = Instantiate(enumOptionPrefab, tabContent);
        enumOption.SetLabel("Enum option");
        enumOption.lockOverlay.SetActive(true);
        AllOptions.Add(enumOption);
        
        var colorOption = Instantiate(colorOptionPrefab, tabContent);
        colorOption.SetLabel("Color option");
        AllOptions.Add(colorOption);
        
        ApplyOptionFilters();
    }

    private void Update()
    {
        /*
        if (Input.anyKeyDown)
        {
            var key = Input.inputString;
            Ls.LogMessage($"Key pressed: {key}");
        }
        */
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Ls.LogMessage($"F2 pressed");
        }
    }
}