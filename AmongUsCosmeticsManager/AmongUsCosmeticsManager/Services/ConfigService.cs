using System;
using System.IO;
using System.Text.Json;
using AmongUsCosmeticsManager.Models.Config;

namespace AmongUsCosmeticsManager.Services;

public class ConfigService
{
    public AppConfig AppConfig { get; private set; } = new();

    public void Load(string? configDir = null)
    {
        var baseDir = configDir ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

        var appConfigPath = Path.Combine(baseDir, "app-config.json");
        if (File.Exists(appConfigPath))
        {
            var json = File.ReadAllText(appConfigPath);
            AppConfig = JsonSerializer.Deserialize(json, AppJsonContext.Default.AppConfig) ?? new AppConfig();
        }
    }
}
