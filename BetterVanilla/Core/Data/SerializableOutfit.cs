using System.IO;
using AmongUs.Data;
using AmongUs.Data.Player;

namespace BetterVanilla.Core.Data;

public sealed class SerializableOutfit
{
    public string Hat { get; }
    public string Skin { get; }
    public string Visor { get; }
    public string Pet { get; }
    public string Nameplate { get; }

    public SerializableOutfit(NetworkedPlayerInfo.PlayerOutfit outfit)
    {
        Hat = outfit.HatId;
        Skin = outfit.SkinId;
        Visor = outfit.VisorId;
        Pet = outfit.PetId;
        Nameplate = outfit.NamePlateId;
    }

    public SerializableOutfit(BinaryReader reader)
    {
        Hat = reader.ReadString();
        Skin = reader.ReadString();
        Visor = reader.ReadString();
        Pet = reader.ReadString();
        Nameplate = reader.ReadString();
    }
    
    public void Write(BinaryWriter writer)
    {
        writer.Write(Hat);
        writer.Write(Skin);
        writer.Write(Visor);
        writer.Write(Pet);
        writer.Write(Nameplate);
    }

    public bool IsEquipped()
    {
        if (DataManager.Player?.Customization == null) return false;
        return IsSame(DataManager.Player.Customization);
    }

    public bool IsSame(SerializableOutfit outfit)
    {
        return Hat == outfit.Hat
            && Skin == outfit.Skin
            && Visor == outfit.Visor
            && Pet == outfit.Pet
            && Nameplate == outfit.Nameplate;
    }

    public bool IsSame(NetworkedPlayerInfo.PlayerOutfit outfit)
    {
        return Hat == outfit.HatId
               && Skin == outfit.SkinId
               && Visor == outfit.VisorId
               && Pet == outfit.PetId
               && Nameplate == outfit.NamePlateId;
    }

    private bool IsSame(PlayerCustomizationData outfit)
    {
        return Hat == outfit.Hat
               && Skin == outfit.Skin
               && Visor == outfit.Visor
               && Pet == outfit.Pet
               && Nameplate == outfit.NamePlate;
    }
}