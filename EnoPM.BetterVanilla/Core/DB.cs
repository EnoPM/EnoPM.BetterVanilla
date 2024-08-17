using System;
using System.IO;
using AmongUs.Data;
using EnoPM.BetterVanilla.Data.Database;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

public static class DB
{
    private static readonly JsonBinaryDatabase<PlayerDatabase> PlayerDatabase;
    private static readonly JsonBinaryDatabase<OutfitsDatabase> OutfitsDatabase;

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
    }

    public static PlayerDatabase Player => PlayerDatabase.Data;
    public static OutfitsDatabase Outfits => OutfitsDatabase.Data;

    public static DressingOutfit SaveCurrentOutfit(string name)
    {
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.Data) return null;
        var outfit = player.Data.DefaultOutfit;
        if (outfit == null) return null;
        var dressingOutfit = new DressingOutfit
        {
            Name = name,
            Hat = outfit.HatId,
            Skin = outfit.SkinId,
            Visor = outfit.VisorId,
            Pet = outfit.PetId,
            Nameplate = outfit.NamePlateId
        };
        Outfits.Outfits.Add(dressingOutfit);
        OutfitsDatabase.Save();

        return dressingOutfit;
    }

    public static void ApplyOutfit(string name)
    {
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.Data) return;
        var outfit = Outfits.Outfits.Find(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        if (outfit == null) return;
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
    
    public static void DeleteOutfit(string name)
    {
        var outfit = Outfits.Outfits.Find(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        Outfits.Outfits.Remove(outfit);
        OutfitsDatabase.Save();
    }

    public static void RefreshPlayerNameAndLevel()
    {
        var client = AmongUsClient.Instance.GetClient(AmongUsClient.Instance.ClientId);
        if (client == null)
        {
            throw new Exception("Unable to find AmongUsClient");
        }
        
        Player.PlayerName = client.PlayerName;
        Player.PlayerLevel = client.PlayerLevel;
        
        PlayerDatabase.Save();
    }
}