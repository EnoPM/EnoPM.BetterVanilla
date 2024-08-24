using System;
using System.Security.Cryptography;
using System.Text;

namespace BetterVanilla.Core.Helpers;

public static class StringUtils
{
    public static string CalculateSHA256(string text)
    {
        var enc = Encoding.UTF8;
        var buffer = enc.GetBytes(text);

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(buffer);

        return BitConverter.ToString(hash).Replace("-", "");
    }
}