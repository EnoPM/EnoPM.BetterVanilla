using System;
using System.IO;
using System.Text;
using System.Text.Json;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Data.Legacy;
using UnityEngine;

namespace BetterVanilla.Core;

public sealed class DatabaseManager
{
    public readonly LocalData Data;
    private readonly string _filePath;
    
    public DatabaseManager()
    {
        var appDataDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
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

        _filePath = Path.Combine(baseDirectory, "db.dat");
        if (!File.Exists(_filePath))
        {
            Data = new LocalData();
        }
        else
        {
            Data = JsonSerializer.Deserialize<LocalData>(File.ReadAllText(_filePath)) ?? new LocalData();
        }
        
        CheckAndMigrateLegacyDatabase(baseDirectory);
    }

    public void Save()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(Data));
    }

    private void CheckAndMigrateLegacyDatabase(string directoryPath)
    {
        var playerFilePath = Path.Combine(directoryPath, "player.dat");
        var outfitsFilePath = Path.Combine(directoryPath, "outfits.dat");
        var presetsFilePath = Path.Combine(directoryPath, "settings.dat");

        if (File.Exists(playerFilePath))
        {
            var legacyPlayerData = JsonSerializer.Deserialize<LegacyPlayerDatabase>(Encoding.UTF8.GetString(File.ReadAllBytes(playerFilePath)));
            Data.PlayerExp = legacyPlayerData.PlayerExp;
            Data.PlayerLevel = legacyPlayerData.PlayerLevel;
            foreach (var featureCode in legacyPlayerData.FeatureCodes)
            {
                Data.FeatureCodes.Add(featureCode);
            }
            Save();
            File.Delete(playerFilePath);
        }

        if (File.Exists(outfitsFilePath))
        {
            var legacyOutfitsData = JsonSerializer.Deserialize<LegacyOutfitsDatabase>(Encoding.UTF8.GetString(File.ReadAllBytes(outfitsFilePath)));
            foreach (var legacyDressingOutfit in legacyOutfitsData.Outfits)
            {
                Data.Outfits.Add(new LocalOutfitData
                {
                    Hat = legacyDressingOutfit.Hat,
                    Skin = legacyDressingOutfit.Skin,
                    Visor = legacyDressingOutfit.Visor,
                    Pet = legacyDressingOutfit.Pet,
                    Nameplate = legacyDressingOutfit.Nameplate
                });
            }
            Save();
            File.Delete(outfitsFilePath);
        }

        if (File.Exists(presetsFilePath))
        {
            File.Delete(presetsFilePath);
        }
    }
}