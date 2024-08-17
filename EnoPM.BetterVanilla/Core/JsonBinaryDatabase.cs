using System.IO;
using System.Text;
using System.Text.Json;

namespace EnoPM.BetterVanilla.Core;

public sealed class JsonBinaryDatabase<T> where T : new()
{
    public readonly T Data;
    private readonly string _filePath;

    private byte[] Serialized => Encoding.UTF8.GetBytes(
        JsonSerializer.Serialize(Data)
    );

    private T Deserialized => JsonSerializer.Deserialize<T>(
        Encoding.UTF8.GetString(
            File.ReadAllBytes(
                _filePath
            )
        )
    );

    public JsonBinaryDatabase(string dbFilePath)
    {
        _filePath = dbFilePath;
        if (!File.Exists(dbFilePath))
        {
            Data = new T();
            Save();
        }
        else
        {
            Data = Deserialized;
        }
    }

    public void Save()
    {
        File.WriteAllBytes(_filePath, Serialized);
    }
}