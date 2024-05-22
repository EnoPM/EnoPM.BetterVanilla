using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(MapBehaviour))]
internal static class MapBehaviourPatches
{
    private static readonly int OutlineSize = Shader.PropertyToID("_Outline");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly Dictionary<byte, SpriteRenderer> AllPlayers = new();
    
    [HarmonyPostfix, HarmonyPatch(nameof(MapBehaviour.FixedUpdate))]
    private static void FixedUpdatePostfix(MapBehaviour __instance)
    {
        if (!Utils.AmDead || MeetingHud.Instance)
        {
            foreach (var kvp in AllPlayers)
            {
                if (kvp.Value)
                {
                    kvp.Value.gameObject.SetActive(false);
                }
            }
            return;
        }
        if (!ShipStatus.Instance) return;
        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null) continue;
            if (!AllPlayers.TryGetValue(pc.PlayerId, out var sr) || !sr)
            {
                if (pc.AmOwner)
                {
                    if (!__instance.HerePoint) continue;
                    sr = __instance.HerePoint;
                }
                else
                {
                    sr = AllPlayers[pc.PlayerId] = Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                    pc.SetPlayerMaterialColors(sr);
                }
                sr.material.SetColor(OutlineColor, Color.white);
            }
            sr.gameObject.SetActive(true);
            var material = sr.material;
            material.SetFloat(OutlineSize, pc.Data.Role && pc.Data.Role.IsImpostor ? 1f : 0f);
            sr.color = pc.Data.IsDead || pc.Data.Disconnected ? Palette.DisabledClear : Palette.EnabledColor;

            var position = pc.transform.position;
            var vector3 = position / ShipStatus.Instance.MapScale;
            vector3.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
            vector3.z = -1f;
            sr.transform.localPosition = vector3;
        }
    }
}