namespace AmongUsDevKit.Utils;

internal static class RandomProvider
{
    private const string CharsUsedInRandomId = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly Random Random = new();

    public static string RandomizeName(string baseName)
    {
        return $"{baseName}_____{CreateRandomId(10)}";
    }

    public static string CreateRandomId(int size) => new(Enumerable.Repeat(CharsUsedInRandomId, size).Select(s => s[Random.Next(s.Length)]).ToArray());
    public static string CreateRandomMethodName() => $"__m_{CreateRandomId(34)}";
    public static string CreateRandomFieldName() => $"__f_{CreateRandomId(34)}";

    public static Version CreateRandomVersion() => new(Random.Next(0, 99), Random.Next(0, 254), Random.Next(0, 254), Random.Next(0, 999999));
}