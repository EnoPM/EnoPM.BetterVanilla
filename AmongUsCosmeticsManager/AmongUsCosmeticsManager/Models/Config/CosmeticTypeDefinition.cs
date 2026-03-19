using System.Collections.Generic;

namespace AmongUsCosmeticsManager.Models.Config;

public class CosmeticTypeDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string PluralLabel { get; set; } = string.Empty;
    public List<PropertyDefinition> Properties { get; set; } = [];
    public List<ResourceSlotDefinition> ResourceSlots { get; set; } = [];
    public List<FrameListDefinition> FrameLists { get; set; } = [];

    public static readonly List<CosmeticTypeDefinition> All =
    [
        new()
        {
            Id = "hat",
            Label = "Hat",
            PluralLabel = "Hats",
            Properties =
            [
                new() { Id = "adaptive", Label = "Adaptive", Default = false },
                new() { Id = "bounce", Label = "Bounce", Default = false },
                new() { Id = "noVisors", Label = "No Visors", Default = false }
            ],
            ResourceSlots =
            [
                new() { Id = "main", Label = "Main", Required = true },
                new() { Id = "flip", Label = "Flip" },
                new() { Id = "back", Label = "Back" },
                new() { Id = "climb", Label = "Climb" }
            ],
            FrameLists =
            [
                new() { Id = "frontAnimation", Label = "Front Animation Frames" },
                new() { Id = "backAnimation", Label = "Back Animation Frames" }
            ]
        },
        new()
        {
            Id = "visor",
            Label = "Visor",
            PluralLabel = "Visors",
            Properties =
            [
                new() { Id = "adaptive", Label = "Adaptive", Default = false },
                new() { Id = "behindHats", Label = "Behind Hats", Default = false }
            ],
            ResourceSlots =
            [
                new() { Id = "main", Label = "Main", Required = true },
                new() { Id = "climb", Label = "Climb" },
                new() { Id = "floor", Label = "Floor" }
            ],
            FrameLists =
            [
                new() { Id = "frontAnimation", Label = "Front Animation Frames" }
            ]
        },
        new()
        {
            Id = "nameplate",
            Label = "Nameplate",
            PluralLabel = "Nameplates",
            Properties =
            [
                new() { Id = "adaptive", Label = "Adaptive", Default = false }
            ],
            ResourceSlots =
            [
                new() { Id = "main", Label = "Main", Required = true }
            ],
            FrameLists = []
        }
    ];
}
