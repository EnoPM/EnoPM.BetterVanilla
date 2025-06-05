using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetterVanilla.CosmeticsCompiler.SpriteSheet;

public class SpriteSheetCreator : IDisposable
{
    private CreateSpriteSheetOptions Options { get; }
    public List<SpriteFile> Files { get; } = [];
    public List<SpriteEntry> Entries { get; } = [];

    public SpriteSheetCreator(CreateSpriteSheetOptions options)
    {
        Options = options;
        if (options.InputPngFilePaths != null)
        {
            foreach (var path in options.InputPngFilePaths)
            {
                AddFile(path);
            }
        }
        if (options.InputPngDirectoryPaths != null)
        {
            foreach (var path in options.InputPngDirectoryPaths)
            {
                AddDirectory(path);
            }
        }
    }

    public void CreateSpriteSheet()
    {
        if (Files.Count == 0)
        {
            Console.WriteLine("No png file to load. Aborting.");
            return;
        }
        Console.Write($"Loading {Files.Count} images...");
        LoadImages();
        PackImages();
    }
    
    public void SaveSpriteSheet()
    {
        var sheetWidth = Entries.Max(e => e.X + e.Width);
        var sheetHeight = Entries.Max(e => e.Y + e.Height);

        using var sheet = new Image<Rgba32>(sheetWidth, sheetHeight);

        foreach (var entry in Entries)
        {
            sheet.Mutate(ctx => ctx.DrawImage(entry.Image, new Point(entry.X, entry.Y), 1f));
        }

        var outputFilePath = Path.Combine(Options.OutputDirectoryPath, $"{Options.Name}.png");
        sheet.Save(outputFilePath);
        Console.WriteLine($"Saved spritesheet at: {outputFilePath}");
    }
    
    public void SaveSpriteSheetMetaTxt()
    {
        var outputFilePath = Path.Combine(Options.OutputDirectoryPath, $"{Options.Name}.txt");
        using var writer = new StreamWriter(outputFilePath);

        foreach (var entry in Entries)
        {
            writer.WriteLine(entry.Name);
            writer.WriteLine("  rotate: false");
            writer.WriteLine($"  xy: {entry.X},{entry.Y}");
            writer.WriteLine($"  size: {entry.Width},{entry.Height}");
            writer.WriteLine($"  orig: {entry.Width},{entry.Height}");
            writer.WriteLine("  offset: 0,0");
            writer.WriteLine("  index: -1");
        }

        Console.WriteLine($"Saved spritesheet metadata at: {outputFilePath}");
    }
    
    private void PackImages()
    {
        var padding = 2;

        // Si l'utilisateur n'a pas défini une largeur, on la calcule automatiquement
        var maxSheetWidth = RoundedUpPowerOfTwo((int)Math.Ceiling(Math.Sqrt(Files.Sum(x => x.Image!.Width * x.Image!.Height))));

        var currentX = 0;
        var currentY = 0;
        var lineHeight = 0;

        foreach (var file in Files.OrderByDescending(x => x.Image!.Height))
        {
            var image = file.Image!;
            if (currentX + image.Width > maxSheetWidth)
            {
                currentX = 0;
                currentY += lineHeight + padding;
                lineHeight = 0;
            }

            Entries.Add(new SpriteEntry
            {
                Name = Path.GetFileNameWithoutExtension(file.Path),
                Image = image,
                X = currentX,
                Y = currentY
            });

            currentX += image.Width + padding;
            lineHeight = Math.Max(lineHeight, image.Height);
        }
    }

    private void LoadImages()
    {
        foreach (var file in Files)
        {
            file.Image = Image.Load(file.Path);
        }
    }

    private void UnloadImages()
    {
        foreach (var file in Files)
        {
            file.Image?.Dispose();
        }
    }

    private void AddFile(string path)
    {
        if (Path.GetExtension(path) != ".png") return;
        if (!File.Exists(path)) return;
        Files.Add(new SpriteFile { Path = path});
    }

    private void AddDirectory(string path)
    {
        if (!Directory.Exists(path)) return;
        foreach (var filePath in Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly))
        {
            AddFile(filePath);
        }
    }
    
    public void Dispose()
    {
        UnloadImages();
    }
    
    private static int RoundedUpPowerOfTwo(int n)
    {
        var power = 1;
        while (power < n) power <<= 1;
        return power;
    }
}