using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace EnoPM.BetterVanilla;

internal static class Utils
{
    private static readonly Dictionary<string, Sprite> Cache = new();
    
    internal static Sprite LoadSpriteFromResource(string path, float pixelsPerUnit)
    {
        return Assembly.GetExecutingAssembly().LoadSpriteFromResources(path, pixelsPerUnit);
    }

    internal static Sprite LoadSpriteFromResource(string path)
    {
        return Assembly.GetExecutingAssembly().LoadSpriteFromResources(path);
    }
    
    internal static bool AmDead => !ModConfigs.CheckAmDead || (PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data != null &&
                                                              PlayerControl.LocalPlayer.Data.IsDead);
    internal static bool AmImpostor => !ModConfigs.CheckAmImpostor || (PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data != null &&
                                                                       PlayerControl.LocalPlayer.Data.Role != null &&
                                                                       PlayerControl.LocalPlayer.Data.Role.IsImpostor);
    internal static bool IsGameStarted => AmongUsClient.Instance && (AmongUsClient.Instance.IsGameStarted || DestroyableSingleton<TutorialManager>.InstanceExists);
    
    public static bool IsLocalPlayer(byte playerId)
    {
        return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.PlayerId == playerId;
    }

    internal static bool IsHost(PlayerControl player)
    {
        return player && AmongUsClient.Instance.HostId == player.OwnerId;
    }

    internal static Tuple<int, int> GetTasksCount(byte playerId)
    {
        var player = GetPlayerById(playerId);
        if (player == null) return new Tuple<int, int>(0, 0);
        return GetTasksCount(player);
    }

    internal static Tuple<int, int> GetTasksCount(NetworkedPlayerInfo player)
    {
        var total = 0;
        var done = 0;
        if (player.Tasks == null) return new Tuple<int, int>(0, 0);
        foreach (var task in player.Tasks)
        {
            total++;
            if (task.Complete)
            {
                done++;
            }
        }

        return new Tuple<int, int>(done, total);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    internal static string Cs(Color c, string s)
    {
        return $"<color=#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}{ToByte(c.a):X2}>{s}</color>";
    }

    internal static NetworkedPlayerInfo GetPlayerById(byte id)
    {
        var pc = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == id);
        if (pc)
        {
            return pc.Data;
        }
        return null;
    }

    internal static string ColorToHex(Color color)
    {
        var r = Mathf.RoundToInt(color.r * 255);
        var g = Mathf.RoundToInt(color.g * 255);
        var b = Mathf.RoundToInt(color.b * 255);
        var a = Mathf.RoundToInt(color.a * 255);
        var hexColor = $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        return hexColor;
    }
    
    public static Color ColorFromHex(string hexColor)
    {
        if (hexColor.StartsWith("#")) hexColor = hexColor.Replace("#", string.Empty);

        var red = 0;
        var green = 0;
        var blue = 0;
        var alpha = 255;

        switch (hexColor.Length)
        {
            case 8:
                red = int.Parse(hexColor.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                green = int.Parse(hexColor.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                blue = int.Parse(hexColor.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                alpha = int.Parse(hexColor.AsSpan(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                break;
            case 6:
                red = int.Parse(hexColor.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                green = int.Parse(hexColor.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                blue = int.Parse(hexColor.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                break;
            case 3:
                red = int.Parse(
                    hexColor[0] + hexColor[0].ToString(),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture);
                green = int.Parse(
                    hexColor[1] + hexColor[1].ToString(),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture);
                blue = int.Parse(
                    hexColor[2] + hexColor[2].ToString(),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture);
                break;
        }

        return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
    }
    
    internal static string CalculateSHA256(string text)
    {
        var enc = Encoding.UTF8;
        var buffer = enc.GetBytes(text);

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(buffer);

        return BitConverter.ToString(hash).Replace("-", "");
    }
}