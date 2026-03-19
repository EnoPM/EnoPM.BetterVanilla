using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AmongUsCosmeticsManager.Services.BundleImport;

internal sealed class ImportSprite
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("w")]
    public int Width { get; set; }

    [JsonPropertyName("h")]
    public int Height { get; set; }
}

internal sealed class ImportAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

internal sealed class ImportHat
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("author")]
    public ImportAuthor? Author { get; set; }

    [JsonPropertyName("a")]
    public bool Bounce { get; set; }

    [JsonPropertyName("b")]
    public bool NoVisors { get; set; }

    [JsonPropertyName("c")]
    public ImportSprite MainResource { get; set; } = null!;

    [JsonPropertyName("d")]
    public ImportSprite PreviewResource { get; set; } = null!;

    [JsonPropertyName("e")]
    public ImportSprite? FlipResource { get; set; }

    [JsonPropertyName("f")]
    public ImportSprite? BackResource { get; set; }

    [JsonPropertyName("g")]
    public ImportSprite? BackFlipResource { get; set; }

    [JsonPropertyName("h")]
    public ImportSprite? ClimbResource { get; set; }

    [JsonPropertyName("i")]
    public List<ImportSprite>? FrontAnimationFrames { get; set; }

    [JsonPropertyName("j")]
    public List<ImportSprite>? BackAnimationFrames { get; set; }
}

internal sealed class ImportVisor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("author")]
    public ImportAuthor? Author { get; set; }

    [JsonPropertyName("a")]
    public ImportSprite MainResource { get; set; } = null!;

    [JsonPropertyName("b")]
    public ImportSprite PreviewResource { get; set; } = null!;

    [JsonPropertyName("c")]
    public ImportSprite? LeftResource { get; set; }

    [JsonPropertyName("d")]
    public ImportSprite? ClimbResource { get; set; }

    [JsonPropertyName("e")]
    public ImportSprite? FloorResource { get; set; }

    [JsonPropertyName("f")]
    public List<ImportSprite>? FrontAnimationFrames { get; set; }

    [JsonPropertyName("g")]
    public bool BehindHats { get; set; }
}

internal sealed class ImportNamePlate
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("author")]
    public ImportAuthor? Author { get; set; }

    [JsonPropertyName("main_resource")]
    public ImportSprite MainResource { get; set; } = null!;
}
