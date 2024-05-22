using System.Collections.Generic;
using TMPro;

namespace EnoPM.BetterVanilla.Extensions;

internal static class PlayerVoteAreaExtensions
{
    private static readonly Dictionary<byte, TextMeshPro> ModdedTexts = new();
    
    private static TextMeshPro GetModdedText(this PlayerVoteArea pva)
    {
        if (ModdedTexts.TryGetValue(pva.TargetPlayerId, out var value) && value) return value;
        if (!pva.NameText) return null;
        ModdedTexts[pva.TargetPlayerId] = UnityEngine.Object.Instantiate(pva.NameText, pva.NameText.transform.parent);
        var pos = ModdedTexts[pva.TargetPlayerId].transform.localPosition;
        pos.y = -0.15f;
        ModdedTexts[pva.TargetPlayerId].transform.localPosition = pos;
        return ModdedTexts[pva.TargetPlayerId];
    }

    public static void ModdedFixedUpdate(this PlayerVoteArea pva)
    {
        var tmp = pva.GetModdedText();
        if (tmp == null) return;
        var player = Utils.GetPlayerById(pva.TargetPlayerId);
        if (player == null) return;
        tmp.text = player.GenerateModdedText();
    }
}