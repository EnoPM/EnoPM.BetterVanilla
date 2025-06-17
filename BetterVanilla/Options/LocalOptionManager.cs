using BetterVanilla.Options.Core;

namespace BetterVanilla.Options;

public sealed class LocalOptionManager : BaseSerializableCategory
{
    protected override string DataFilePath { get; }

    public LocalOptionManager(string dataFilePath)
    {
        DataFilePath = dataFilePath;
    }
}