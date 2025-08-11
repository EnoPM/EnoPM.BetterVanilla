using BetterVanilla.GeneratedRuntime;
using Hazel;

namespace BetterVanilla.Core.Data;

public sealed class BetterVanillaHandshake
{
    public static readonly BetterVanillaHandshake Local = new();
    public static readonly BetterVanillaHandshake TooOld = new("v1.x.x");
    
    public string Version { get; }
    public string Guid { get; }
    
    private BetterVanillaHandshake()
    {
        Version = $"v{GeneratedProps.Version}";
        Guid = typeof(BetterVanillaPlugin).Module.ModuleVersionId.ToString();
    }

    private BetterVanillaHandshake(string version)
    {
        Version = version;
        Guid = "too-old";
    }

    public BetterVanillaHandshake(MessageReader reader)
    {
        Version = reader.ReadString();
        Guid = reader.ReadString();
    }

    public void Serialize(MessageWriter writer)
    {
        writer.Write(Version);
        writer.Write(Guid);
    }
}