using System.IO;
using System.IO.Compression;

namespace BetterVanilla.Cosmetics.Api.Core;

public static class ByteCompressor
{
    public static byte[] Compress(byte[] data)
    {
        using var ms = new MemoryStream();
        using (var brotli = new BrotliStream(ms, CompressionLevel.SmallestSize, leaveOpen: true))
            brotli.Write(data, 0, data.Length);
        return ms.ToArray();
    }

    public static byte[] Decompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return output.ToArray();
    }
}