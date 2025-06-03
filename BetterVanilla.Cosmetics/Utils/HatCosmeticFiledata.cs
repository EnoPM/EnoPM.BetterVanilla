using System.Collections.Generic;
using System.Linq;

namespace BetterVanilla.Cosmetics.Utils;

public sealed class HatCosmeticFiledata
{
    public readonly string Key;
    public readonly List<string> Options = [];
    public readonly string FullPath;

    public HatCosmeticFiledata(string filename, bool fromDisk)
    {
        FullPath = filename;
        var namePart = fromDisk
            ? filename[(filename.LastIndexOf('\\') + 1)..].Split('.')[0]
            : filename.Split('.')[3];

        var parts = namePart.Split('_');
        Key = parts[0];
        Options = parts
            .Skip(1) // key
            .ToList();
    }
}