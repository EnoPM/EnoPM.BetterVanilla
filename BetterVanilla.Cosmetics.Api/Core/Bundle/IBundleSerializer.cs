using System.IO;

namespace BetterVanilla.Cosmetics.Api.Core.Bundle;

internal interface IBundleSerializer
{
    public CosmeticBundle Deserialize(BinaryReader reader);
}