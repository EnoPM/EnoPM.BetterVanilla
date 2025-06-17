using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BetterVanilla.Core;

namespace BetterVanilla.Options.Core;

public abstract class BaseSerializableCategory
{
    public BaseSerializableOption[] Options => AllOptions.ToArray();
    protected abstract string DataFilePath { get; }
    private List<BaseSerializableOption> AllOptions { get; } = [];

    public void Add<T>(T option)
        where T : BaseSerializableOption
    {
        AllOptions.Add(option);
    }

    public void Save()
    {
        using var stream = File.Create(DataFilePath);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(AllOptions.Count);

        foreach (var option in AllOptions)
        {
            option.Serialize(writer);
        }
    }

    public void Load()
    {
        if (!File.Exists(DataFilePath))
        {
            return;
        }

        using var stream = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);

        var optionsCount = reader.ReadInt32();
        for (var i = 0; i < optionsCount; i++)
        {
            var name = reader.ReadString();
            var option = AllOptions.FirstOrDefault(x => x.Name == name);
            if (option == null)
            {
                Ls.LogWarning($"Unknown serialized option {name}");
                continue;
            }
            option.Deserialize(reader);
        }
    }
}