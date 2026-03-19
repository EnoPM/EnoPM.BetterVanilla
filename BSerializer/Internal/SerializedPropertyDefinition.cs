namespace BSerializer.Internal;

internal sealed class SerializedPropertyDefinition(uint id, uint typeId, string name)
{
    public uint Id { get; } = id;
    public uint TypeId { get; } = typeId;
    public string Name { get; } = name;
}