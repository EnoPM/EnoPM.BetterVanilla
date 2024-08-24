using System.Collections.Generic;

namespace BetterVanilla.Core.Extensions;

public static class EnumerableExtensions
{
    public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this IEnumerable<T> enumerable)
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in enumerable)
        {
            list.Add(item);
        }
        return list;
    }
}