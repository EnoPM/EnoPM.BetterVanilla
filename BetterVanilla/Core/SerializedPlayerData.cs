using System;
using System.IO;

namespace BetterVanilla.Core;

public sealed class SerializedPlayerData
{
    public static readonly SerializedPlayerData Default = new();
    
    private uint _xp;
    private uint _level;
    private bool _checkPrerelease;

    public uint Xp
    {
        get => _xp;
        set
        {
            if (_xp == value) return;
            _xp = value;
            Save();
        }
    }

    [Obsolete("Use vanilla level instead")]
    public uint Level
    {
        get => _level;
        set
        {
            if (_level == value) return;
            _level = value;
            Save();
        }
    }

    public bool CheckPrerelease
    {
        get => _checkPrerelease;
        set
        {
            if (_checkPrerelease == value) return;
            _checkPrerelease = value;
            Save();
        }
    }

    public SerializedPlayerData()
    {
        if (!File.Exists(ModPaths.PlayerDataFile))
        {
            _xp = 0;
            _level = 0;
            return;
        }
        using var file = File.OpenRead(ModPaths.PlayerDataFile);
        using var reader = new BinaryReader(file);
        _xp = reader.ReadUInt32();
        _level = reader.ReadUInt32();
        _checkPrerelease = reader.BaseStream.Position < reader.BaseStream.Length && reader.ReadBoolean();
    }

    private void Save()
    {
        using var file = File.Create(ModPaths.PlayerDataFile);
        using var writer = new BinaryWriter(file);
        writer.Write(_xp);
        writer.Write(_level);
        writer.Write(_checkPrerelease);
    }
}