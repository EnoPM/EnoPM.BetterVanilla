using System.Collections.Generic;

namespace BSerializer.Internal;

internal sealed class SerializedTypeDefinition(uint id, string name)
{
    public uint Id { get; } = id;
    public string Name { get; } = name;
    public List<SerializedPropertyDefinition> Properties { get; } = [];
}