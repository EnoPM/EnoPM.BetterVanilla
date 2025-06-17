namespace BetterVanilla.Options.Core;

public abstract class LocalSerializableOption : BaseSerializableOption
{
    protected LocalSerializableOption(string name, string? title = null) : base(name, title)
    {
    }
}