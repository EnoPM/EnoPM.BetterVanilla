using System.IO;
using AmongUs.GameOptions;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Options;

namespace BetterVanilla.Core;

public sealed class SerializableGamePreset
{
    public string Name { get; }
    
    private byte[] RawVanillaOptions { get; }
    private byte[] RawHostOptions { get; }

    public SerializableGamePreset(string name)
    {
        Name = name;
        RawVanillaOptions = ByteCompressor.Compress(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.CurrentGameOptions, false));
        RawHostOptions = HostOptions.Default.ToBytes();
    }

    public SerializableGamePreset(BinaryReader reader)
    {
        Name = reader.ReadString();
        
        var vanillaOptionsSize = reader.ReadInt32();
        RawVanillaOptions = reader.ReadBytes(vanillaOptionsSize);
        
        var hostOptionsSize = reader.ReadInt32();
        RawHostOptions = reader.ReadBytes(hostOptionsSize);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(RawVanillaOptions.Length);
        writer.Write(RawVanillaOptions);
        writer.Write(RawHostOptions.Length);
        writer.Write(RawHostOptions);
    }

    public void Apply()
    {
        GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(ByteCompressor.Decompress(RawVanillaOptions));
        
        GameOptionsManager.Instance.CurrentGameOptions.SetBool(BoolOptionNames.IsDefaults, false);
        GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
        GameManager.Instance.LogicOptions.SyncOptions();
        
        HostOptions.Default.FromBytes(RawHostOptions);
        HostOptions.Default.ShareAllOptions();
    }
}