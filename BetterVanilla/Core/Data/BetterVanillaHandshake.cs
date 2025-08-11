using BetterVanilla.GeneratedRuntime;
using Hazel;

namespace BetterVanilla.Core.Data;

public sealed class BetterVanillaHandshake
{
    public static readonly BetterVanillaHandshake Local = new();
    
    public string Version { get; }
    public string Guid { get; }
    
    public BetterVanillaHandshake()
    {
        Version = GeneratedProps.Version;
        Guid = typeof(BetterVanillaPlugin).Module.ModuleVersionId.ToString();
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