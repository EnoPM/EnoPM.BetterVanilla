using System;
using System.IO;
using AmongUs.GameOptions;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;

namespace BetterVanilla.Core;

public sealed class SerializableGamePreset
{
    public string Name { get; }
    private VanillaOptionPreset VanillaOptions { get; }
    private BetterOptionPreset BetterOptions { get; }

    public SerializableGamePreset(string name)
    {
        Name = name;
        var vanillaOptions = GameOptionsManager.Instance.CurrentGameOptions.TryCast<NormalGameOptionsV09>();
        if (vanillaOptions == null)
        {
            throw new Exception($"Unable to cast normalGameHostOptions to {nameof(NormalGameOptionsV09)}");
        }
        VanillaOptions = new VanillaOptionPreset(vanillaOptions);
        BetterOptions = new BetterOptionPreset();
    }

    public SerializableGamePreset(BinaryReader reader)
    {
        Name = reader.ReadString();
        VanillaOptions = new VanillaOptionPreset(reader);
        BetterOptions = new BetterOptionPreset(reader);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Name);
        VanillaOptions.Serialize(writer);
        BetterOptions.Serialize(writer);
    }

    public void Apply()
    {
        var options = GameOptionsManager.Instance.CurrentGameOptions.TryCast<NormalGameOptionsV09>();
        if (options == null)
        {
            throw new Exception($"Unable to cast CurrentGameOptions to {nameof(NormalGameOptionsV09)}");
        }
        VanillaOptions.Apply(GameOptionsManager.Instance.CurrentGameOptions);
        
        GameOptionsManager.Instance.CurrentGameOptions.SetBool(BoolOptionNames.IsDefaults, false);
        GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
        GameManager.Instance.LogicOptions.SyncOptions();
        
        BetterOptions.Apply();
        HostOptions.Default.ShareAllOptions();
    }
}