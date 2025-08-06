using System.Collections.Generic;
using System.Linq;

namespace BetterVanilla.Core;

public sealed class UiInteractionBlocker
{
    private static readonly List<UiInteractionBlocker> AllBlockers = [];

    public static bool ShouldBlock()
    {
        return AllBlockers.Any(x => x._blocked);
    }

    private bool _blocked;

    public UiInteractionBlocker()
    {
        _blocked = false;
        AllBlockers.Add(this);
    }

    public void Block()
    {
        _blocked = true;
    }

    public void Unblock()
    {
        _blocked = false;
    }
}