using System;
using System.IO;
using System.Linq;
using AmongUs.Data;
using EnoPM.BetterVanilla.Core.Data.Database;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

public static class DB
{
    private static readonly JsonBinaryDatabase<PlayerDatabase> PlayerDatabase;
    private static readonly JsonBinaryDatabase<OutfitsDatabase> OutfitsDatabase;
    private static readonly JsonBinaryDatabase<PresetsDatabase> PresetsDatabase;

    static DB()
    {
        var appDataDirectory = Path.GetDirectoryName(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        );
        if (appDataDirectory == null)
        {
            throw new Exception("Unable to locate appData directory");
        }
        
        var baseDirectory = Path.Combine(
            appDataDirectory,
            "LocalLow",
            Application.companyName,
            Application.productName,
            "BetterVanilla"
        );

        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }
        
        PlayerDatabase = new JsonBinaryDatabase<PlayerDatabase>(Path.Combine(baseDirectory, "player.dat"));
        OutfitsDatabase = new JsonBinaryDatabase<OutfitsDatabase>(Path.Combine(baseDirectory, "outfits.dat"));
        PresetsDatabase = new JsonBinaryDatabase<PresetsDatabase>(Path.Combine(baseDirectory, "settings.dat"));
        
        FeatureLocker.Reload();
    }

    public static PlayerDatabase Player => PlayerDatabase.Data;
    public static OutfitsDatabase Outfits => OutfitsDatabase.Data;
    public static PresetsDatabase Presets => PresetsDatabase.Data;

    public static DressingOutfit SaveCurrentOutfit()
    {
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.Data) return null;
        var outfit = player.Data.DefaultOutfit;
        if (outfit == null) return null;
        var dressingOutfit = new DressingOutfit
        {
            Hat = outfit.HatId,
            Skin = outfit.SkinId,
            Visor = outfit.VisorId,
            Pet = outfit.PetId,
            Nameplate = outfit.NamePlateId
        };
        if (Outfits.Outfits.Any(x => x.IsSame(dressingOutfit)))
        {
            return null;
        }
        Outfits.Outfits.Add(dressingOutfit);
        OutfitsDatabase.Save();

        return dressingOutfit;
    }

    public static void ApplyOutfit(DressingOutfit outfit)
    {
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.Data) return;
        DataManager.Player.Customization.Hat = outfit.Hat;
        player.RpcSetHat(outfit.Hat);
        DataManager.Player.Customization.Skin = outfit.Skin;
        player.RpcSetSkin(outfit.Skin);
        DataManager.Player.Customization.Visor = outfit.Visor;
        player.RpcSetVisor(outfit.Visor);
        DataManager.Player.Customization.Pet = outfit.Pet;
        player.RpcSetPet(outfit.Pet);
        DataManager.Player.Customization.NamePlate = outfit.Nameplate;
        player.RpcSetNamePlate(outfit.Nameplate);
    }
    
    public static void DeleteOutfit(DressingOutfit outfitToDelete)
    {
        var outfit = Outfits.Outfits.Find(x => x.IsSame(outfitToDelete));
        Outfits.Outfits.Remove(outfit);
        OutfitsDatabase.Save();
    }

    public static void RefreshPlayerName()
    {
        var client = AmongUsClient.Instance.GetClient(AmongUsClient.Instance.ClientId);
        if (client == null)
        {
            throw new Exception("Unable to find AmongUsClient");
        }
        
        Player.PlayerName = client.PlayerName;
        
        PlayerDatabase.Save();
    }

    public static void SavePresets() => PresetsDatabase.Save();
    public static void SavePlayer() => PlayerDatabase.Save();
}