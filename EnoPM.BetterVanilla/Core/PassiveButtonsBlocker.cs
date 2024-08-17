using System.Collections.Generic;
using System.Linq;

namespace EnoPM.BetterVanilla.Core;

public sealed class PassiveButtonsBlocker
{
    private static readonly List<PassiveButtonsBlocker> AllBlockers = [];

    public static bool ShouldBlock()
    {
        return AllBlockers.Any(x => x._blocked);
    }

    private bool _blocked;

    public PassiveButtonsBlocker()
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