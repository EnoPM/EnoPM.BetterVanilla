using System.Collections.Generic;
using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Core.Data;

namespace BetterVanilla.Core.Data;

public sealed class FeaturesRegistry
{
    [JsonPropertyName("contributors")]
    public List<string> ContributorFriendCodes { get; set; } = null!;
    
    [JsonPropertyName("features")]
    public Dictionary<string, List<string>> FeatureHashPermissions { get; set; } = null!;
    
    [JsonPropertyName("task_durations")]
    public Dictionary<TaskTypes, AutoTaskData> TaskDurations { get; set; } = null!;
    
    [JsonPropertyName("sponsor_cosmetics")]
    public List<string> SponsorCosmetics { get; set; } = [];
    
    [JsonPropertyName("hashed_cosmetics")]
    public Dictionary<string, List<string>> HashedCosmetics { get; set; } = new();
    
    [JsonPropertyName("cosmetics_bundle_versions")]
    public List<CosmeticsBundleVersion> CosmeticsBundleVersions { get; set; } = null!;
    
}