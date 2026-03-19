using System.Collections.Generic;

namespace BSerializer.Internal;

internal sealed class SerializedDocument
{
    public List<SerializedTypeDefinition> Definitions { get; } = [];
    public byte[] Data = [];
}