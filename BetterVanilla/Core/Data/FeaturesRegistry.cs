using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data;

public sealed class FeaturesRegistry
{
    [JsonPropertyName("contributors")]
    public List<string> ContributorFriendCodes { get; set; }
    
    [JsonPropertyName("features")]
    public Dictionary<string, List<string>> FeatureHashPermissions { get; set; }
    
    [JsonPropertyName("task_durations")]
    public Dictionary<TaskTypes, AutoTaskData> TaskDurations { get; set; }
}