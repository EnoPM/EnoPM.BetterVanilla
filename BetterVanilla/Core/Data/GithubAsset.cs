﻿using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data;

// ReSharper disable once ClassNeverInstantiated.Global
public class GithubAsset
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("size")]
    public int Size { get; set; }
    
    [JsonPropertyName("browser_download_url")]
    public string DownloadUrl { get; set; }
}