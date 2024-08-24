﻿using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Options;

namespace BetterVanilla.Core;

public sealed class HostOptionsHolder
{
    private readonly HostCategory _category;
    
    public readonly BoolHostOption AllowTeamPreference;
    public readonly FloatHostOption PolusReactorCountdown;

    public HostOptionsHolder()
    {
        _category = new HostCategory("Better Vanilla");

        AllowTeamPreference = _category.CreateBool("TeamPreferenceAllowed", "Allow Team Preferences", true);
        PolusReactorCountdown = _category.CreateFloat("PolusReactorCountdown", "Polus Reactor Countdown", 60f, 1f, new FloatRange(15f, 120f), "0.0", false, NumberSuffixes.Seconds);
    }

    public void ShareAllOptions()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        foreach (var option in _category.AllOptions)
        {
            PlayerControl.LocalPlayer.RpcShareHostOption(option);
        }
    }
}