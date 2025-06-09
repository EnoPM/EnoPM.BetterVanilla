namespace BetterVanilla.Cosmetics.Api.Bundle;

internal interface IBundleSerializer
{
    public CosmeticBundle Deserialize(BinaryReader reader);
}