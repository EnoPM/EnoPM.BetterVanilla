using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Components;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MapBehaviourExtensions
{
    private static readonly int OutlineSize = Shader.PropertyToID("_Outline");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly Dictionary<byte, SpriteRenderer> AllPlayers = new();
    private static readonly Dictionary<int, SpriteRenderer> AllVents = new();
    private static readonly List<Color> VentNetworkColors = [Color.yellow, Color.cyan, Palette.Orange, Palette.AcceptedGreen, Color.magenta];

    private static MapOptions.Modes MapMode { get; set; }

    static MapBehaviourExtensions()
    {
        GameEventManager.GameStarted += Clear;
    }

    private static void Clear()
    {
        foreach (var (_, sr) in AllPlayers)
        {
            if (sr == null) continue;
            Object.Destroy(sr);
        }
        AllPlayers.Clear();
        foreach (var (_, sr) in AllVents)
        {
            if (sr == null) continue;
            Object.Destroy(sr);
        }
        AllVents.Clear();
    }

    public static void BetterShow(this MapBehaviour _, MapOptions opts)
    {
        MapMode = opts.Mode;
    }

    private static void DisableAllPlayerIcons(this MapBehaviour _)
    {
        foreach (var (_, sr) in AllPlayers)
        {
            if (!sr) continue;
            sr.gameObject.SetActive(false);
        }
    }

    private static void UpdateOrCreateAllPlayerIcons(this MapBehaviour mapBehaviour)
    {
        var shipStatus = ShipStatus.Instance;
        if (!shipStatus) return;
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
            sr.gameObject.SetActive(!pc.Data.Disconnected);
            var material = sr.material;
            material.SetFloat(OutlineSize, pc.Data.Role && pc.Data.Role.IsImpostor ? 1f : 0f);
            sr.color = pc.Data.IsDead || pc.Data.Disconnected ? Palette.DisabledClear : Palette.EnabledColor;

            var position = pc.transform.position;
            var vector3 = position / shipStatus.MapScale;
            vector3.x *= Mathf.Sign(shipStatus.transform.localScale.x);
            vector3.z = -2f;
            sr.transform.localPosition = vector3;
        }
    }

    private static bool IsSameNetwork(Vent a, Vent b, HashSet<int>? visited = null)
    {
        if (a.Id == b.Id) return true;

        if (visited == null)
        {
            visited = [];
        }

        if (!visited.Add(a.Id))
        {
            return false;
        }
        
        foreach (var nearbyVent in a.NearbyVents)
        {
            if (!nearbyVent) continue;
            if (IsSameNetwork(nearbyVent, b, visited)) return true;
        }

        return false;
    }

    private static List<List<Vent>> GetVentNetworks()
    {
        var networks = new List<List<Vent>>();

        foreach (var vent in ShipStatus.Instance.AllVents)
        {
            var network = networks.Find(x => x.Any(y => IsSameNetwork(vent, y)));
            if (network == null)
            {
                network = [];
                networks.Add(network);
            } 
            network.Add(vent);
        }

        return networks;
    }

    private static void UpdateOrCreateAllVentIcons(this MapBehaviour mapBehaviour)
    {
        var shipStatus = ShipStatus.Instance;
        if (!shipStatus) return;
        if (AllVents.Count > 0)
        {
            foreach (var (_, sr) in AllVents)
            {
                sr.gameObject.SetActive(true);
            }
            return;
        }
        var networks = GetVentNetworks();
        for (var i = 0; i < networks.Count; i++)
        {
            var network = networks[i];
            var color = VentNetworkColors[i % VentNetworkColors.Count];
            foreach (var vent in network)
            {
                if (AllVents.ContainsKey(vent.Id))
                {
                    Ls.LogWarning($"Trying to create already created vent {vent.Id} in network {i}");
                    continue;
                }
                var sr = AllVents[vent.Id] = Object.Instantiate(mapBehaviour.HerePoint, mapBehaviour.HerePoint.transform.parent);
                sr.sprite = BetterVanillaManager.Instance.VentSprite;
                sr.gameObject.SetActive(true);
                sr.color = color;
            
                var position = vent.transform.position;
                var vector3 = position / shipStatus.MapScale;
                vector3.x *= Mathf.Sign(shipStatus.transform.localScale.x);
                vector3.y += 0.05f;
                vector3.z = -1f;
                sr.transform.localPosition = vector3;
            }
        }
    }

    private static void DisableAllVentIcons(this MapBehaviour _)
    {
        foreach (var (_, sr) in AllVents)
        {
            if (!sr) continue;
            sr.gameObject.SetActive(false);
        }
    }

    public static void BetterFixedUpdate(this MapBehaviour mapBehaviour)
    {
        if (LocalConditions.ShouldRevealVentPositionsInMap())
        {
            mapBehaviour.UpdateOrCreateAllVentIcons();
        }
        else
        {
            mapBehaviour.DisableAllVentIcons();
        }
        if (LocalConditions.ShouldRevealPlayerPositionsInMap(MapMode))
        {
            mapBehaviour.UpdateOrCreateAllPlayerIcons();
        }
        else
        {
            mapBehaviour.DisableAllPlayerIcons();
        }
    }
}