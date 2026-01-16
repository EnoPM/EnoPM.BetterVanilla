using System.Collections;
using System.Collections.Generic;
using BetterVanilla.Components;
using UnityEngine;

namespace BetterVanilla.Core;

public static class PlayerAnonymizer
{
    private static Dictionary<byte, CachedAnonymizedPlayerData> RealOutfits { get; } = new();
    
    public static bool IsActive { get; private set; }

    public static IEnumerator CoEnable(float delay)
    {
        yield return new WaitForSeconds(delay);
        Enable();
    }

    public static void Enable()
    {
        if (IsActive) return;
        foreach (var player in BetterVanillaManager.Instance.AllPlayers)
        {
            RealOutfits[player.Player.PlayerId] = new CachedAnonymizedPlayerData(player);
            AnonymizePlayer(player);
        }
        IsActive = true;
    }

    private static void AnonymizePlayer(BetterPlayerControl player)
    {
        var color = Palette.PreviewGreenColorId;
        player.Player.SetName("***");
        player.Player.SetColor(color);
        player.Player.SetHat(HatData.EmptyId, color);
        player.Player.SetVisor(VisorData.EmptyId, color);
        player.Player.SetSkin(SkinData.EmptyId, color);
        player.Player.SetPet(PetData.EmptyId, color);
    }

    public static IEnumerator CoDisable(float delay)
    {
        yield return new WaitForSeconds(delay);
        Disable();
    }

    public static void Disable()
    {
        if (!IsActive) return;
        foreach (var (playerId, outfit) in RealOutfits)
        {
            var player = BetterVanillaManager.Instance.GetPlayerById(playerId);
            if (player == null)
            {
                Ls.LogWarning($"Unable to restore anonymized player by playerId: {playerId}");
                continue;
            }
            outfit.Restore(player);
        }
        RealOutfits.Clear();
        IsActive = false;
    }
    
    private class CachedAnonymizedPlayerData
    {
        public string PlayerName { get; }
        public int Color { get; }
        public string Hat { get; }
        public string Visor { get; }
        public string Skin { get; }
        public string Pet { get; }

        public CachedAnonymizedPlayerData(BetterPlayerControl player)
        {
            PlayerName = player.Player.Data.PlayerName;
            Color = player.Player.Data.DefaultOutfit.ColorId;
            Hat = player.Player.Data.DefaultOutfit.HatId;
            Visor = player.Player.Data.DefaultOutfit.VisorId;
            Skin = player.Player.Data.DefaultOutfit.SkinId;
            Pet = player.Player.Data.DefaultOutfit.PetId;
        }

        public void Restore(BetterPlayerControl player)
        {
            player.Player.SetName(PlayerName);
            player.Player.SetColor(Color);
            player.Player.SetHat(Hat, Color);
            player.Player.SetVisor(Visor, Color);
            player.Player.SetSkin(Skin, Color);
            player.Player.SetPet(Pet, Color);
        }
    }
}