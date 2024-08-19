using System.Collections.Generic;
using TMPro;

namespace EnoPM.BetterVanilla.Core.Extensions;

internal static class PlayerControlExtensions
{
    private static readonly Dictionary<int, TextMeshPro> ModdedTexts = new();

    private static TextMeshPro GetModdedText(this PlayerControl pc)
    {
        if (ModdedTexts.TryGetValue(pc.OwnerId, out var value) && value) return value;
        if (!pc.cosmetics) return null;
        ModdedTexts[pc.OwnerId] = UnityEngine.Object.Instantiate(pc.cosmetics.nameText, pc.cosmetics.nameText.transform.parent);
        var pos = ModdedTexts[pc.OwnerId].transform.localPosition;
        pos.y = 0.2f;
        ModdedTexts[pc.OwnerId].transform.localPosition = pos;
        ModdedTexts[pc.OwnerId].gameObject.SetActive(true);
        return ModdedTexts[pc.OwnerId];
    }
    
    internal static void ModdedFixedUpdate(this PlayerControl pc)
    {
        if (pc.Data == null || !pc.cosmetics) return;
        var tmp = pc.GetModdedText();
        if (!tmp) return;
        var isActive = Utils.AmDead || pc.AmOwner || !Utils.IsGameStarted;
        tmp.gameObject.SetActive(isActive);
        if (!isActive) return;
        var text = pc.Data.GenerateModdedText();
        if (tmp.text != text)
        {
            tmp.SetText(text);
        }
    }
}