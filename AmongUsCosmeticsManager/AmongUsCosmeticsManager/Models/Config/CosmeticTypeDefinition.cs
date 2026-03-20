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
                new() { Id = "preview", Label = "Preview", AnimationId = "previewAnimation" },
                new() { Id = "front", Label = "Front", Required = true, AnimationId = "frontAnimation" },
                new() { Id = "flip", Label = "Flip", AnimationId = "flipAnimation" },
                new() { Id = "back", Label = "Back", AnimationId = "backAnimation" },
                new() { Id = "backFlip", Label = "Back Flip", AnimationId = "backFlipAnimation" },
                new() { Id = "climb", Label = "Climb", AnimationId = "climbAnimation" }
            ],
            FrameLists =
            [
                new() { Id = "previewAnimation", Label = "Preview Animation" },
                new() { Id = "frontAnimation", Label = "Front Animation" },
                new() { Id = "flipAnimation", Label = "Flip Animation" },
                new() { Id = "backAnimation", Label = "Back Animation" },
                new() { Id = "backFlipAnimation", Label = "Back Flip Animation" },
                new() { Id = "climbAnimation", Label = "Climb Animation" }
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
                new() { Id = "preview", Label = "Preview", AnimationId = "previewAnimation" },
                new() { Id = "front", Label = "Front", Required = true, AnimationId = "frontAnimation" },
                new() { Id = "left", Label = "Left", AnimationId = "leftAnimation" },
                new() { Id = "floor", Label = "Floor", AnimationId = "floorAnimation" }
            ],
            FrameLists =
            [
                new() { Id = "previewAnimation", Label = "Preview Animation" },
                new() { Id = "frontAnimation", Label = "Front Animation" },
                new() { Id = "leftAnimation", Label = "Left Animation" },
                new() { Id = "floorAnimation", Label = "Floor Animation" }
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
                new() { Id = "preview", Label = "Preview", AnimationId = "previewAnimation" },
                new() { Id = "resource", Label = "Resource", Required = true, AnimationId = "resourceAnimation" }
            ],
            FrameLists =
            [
                new() { Id = "previewAnimation", Label = "Preview Animation" },
                new() { Id = "resourceAnimation", Label = "Resource Animation" }
            ]
        }
    ];
}
