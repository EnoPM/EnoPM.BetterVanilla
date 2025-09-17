using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.ToolsLib.Converters;

[UsedImplicitly]
public sealed class JsonVersionConverter : JsonConverter<Version>
{
    public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Version.Parse(reader.GetString()!);
    }
    
    public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}