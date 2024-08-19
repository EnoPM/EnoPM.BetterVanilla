using System.IO;

namespace EnoPM.BetterVanilla.Core.Extensions;

internal static class StreamExtensions
{
    internal static Il2CppSystem.IO.Stream WrapToIl2Cpp(this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var data = memoryStream.ToArray();

        return new Il2CppSystem.IO.MemoryStream(data);
    }
}