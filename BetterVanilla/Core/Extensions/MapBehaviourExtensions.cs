using System.Collections.Generic;
using BetterVanilla.Components;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MapBehaviourExtensions
{
    private static readonly int OutlineSize = Shader.PropertyToID("_Outline");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly Dictionary<byte, SpriteRenderer> AllPlayers = new();
    
    private static MapOptions.Modes MapMode { get; set; }

    public static void BetterShow(this MapBehaviour _, MapOptions opts)
    {
        MapMode = opts.Mode;
    }
    
    public static void BetterFixedUpdate(this MapBehaviour mapBehaviour)
    {
        if (MapMode != MapOptions.Modes.Normal || !BetterVanillaManager.Instance.LocalOptions.DisplayPlayersInMapAfterDeath.Value || MeetingHud.Instance || ConditionUtils.AmAlive() || ConditionUtils.AmImpostor())
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
                    if (!mapBehaviour.HerePoint) continue;
                    sr = mapBehaviour.HerePoint;
                }
                else
                {
                    sr = AllPlayers[pc.PlayerId] = Object.Instantiate(mapBehaviour.HerePoint, mapBehaviour.HerePoint.transform.parent);
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