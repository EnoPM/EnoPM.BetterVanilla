using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetterVanilla.CosmeticsCompiler.Core;

public static class CosmeticPreviewGenerator
{
    private const int PreviewSize = 180;
    
    private static Image<Rgba32> CropToSquareWithContent(Image<Rgba32> source)
    {
        // Trouver les limites du contenu non-transparent
        var bounds = FindContentBounds(source);

        if (bounds != Rectangle.Empty)
        {
            // Calculer la taille carrée basée sur la plus grande dimension
            var squareSize = Math.Max(bounds.Width, bounds.Height);
        
            // Centrer le contenu dans le carré
            var offsetX = (squareSize - bounds.Width) / 2;
            var offsetY = (squareSize - bounds.Height) / 2;
        
            // Créer l'image carrée finale
            var result = new Image<Rgba32>(squareSize, squareSize);
        
            // Copier le contenu centré
            result.Mutate(ctx => 
            {
                // Découper la région de contenu de l'image source
                using var croppedSource = source.Clone(x => x.Crop(bounds));
                ctx.DrawImage(croppedSource, new Point(offsetX, offsetY), 1f);
            });
            
            // Redimensionner intelligemment à 150x150px sans étirer
            return ResizeToFit(result, PreviewSize, PreviewSize);
        }
        
        var empty = new Image<Rgba32>(PreviewSize, PreviewSize);
        source.Dispose();
        return empty;
    }
    
    private static Rectangle FindContentBounds(Image<Rgba32> image)
    {
        int minX = image.Width, maxX = -1;
        int minY = image.Height, maxY = -1;
        
        // Parcourir tous les pixels pour trouver les limites du contenu
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                // Vérifier si le pixel n'est pas transparent (alpha > 0)
                if (pixel.A <= 0) continue;
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }
        }
        
        // Si aucun pixel non-transparent trouvé
        if (maxX == -1)
        {
            return Rectangle.Empty;
        }
            
        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }
    
    private static Image<Rgba32> ResizeToFit(Image<Rgba32> source, int targetWidth, int targetHeight)
    {
        // Si l'image source est déjà plus petite ou égale, on la centre sans redimensionner
        if (source.Width <= targetWidth && source.Height <= targetHeight)
        {
            var result = new Image<Rgba32>(targetWidth, targetHeight);
            var offsetX = (targetWidth - source.Width) / 2;
            var offsetY = (targetHeight - source.Height) / 2;
            
            result.Mutate(ctx => ctx.DrawImage(source, new Point(offsetX, offsetY), 1f));
            source.Dispose();
            return result;
        }
        
        // Calculer le facteur de mise à l'échelle en préservant les proportions
        var scaleX = (float)targetWidth / source.Width;
        var scaleY = (float)targetHeight / source.Height;
        var scale = Math.Min(scaleX, scaleY); // Prendre le plus petit pour s'assurer que tout rentre
        
        var newWidth = (int)(source.Width * scale);
        var newHeight = (int)(source.Height * scale);
        
        // Redimensionner l'image source
        source.Mutate(x => x.Resize(newWidth, newHeight));
        
        // Créer le canvas final et centrer l'image redimensionnée
        var final = new Image<Rgba32>(targetWidth, targetHeight);
        var finalOffsetX = (targetWidth - newWidth) / 2;
        var finalOffsetY = (targetHeight - newHeight) / 2;
        
        final.Mutate(ctx => ctx.DrawImage(source, new Point(finalOffsetX, finalOffsetY), 1f));
        source.Dispose();
        return final;
    }
    
    private static Image<Rgba32> CreatePreviewImageFrom(params Image<Rgba32>[] images)
    {
        if (images.Length == 0)
        {
            throw new ArgumentException("At least one image is required", nameof(images));
        }

        // Calcul de la taille finale basée sur l'image la plus grande
        var maxWidth = images.Max(img => img.Width);
        var maxHeight = images.Max(img => img.Height);

        // Création de l'image de résultat avec fond transparent
        var result = new Image<Rgba32>(maxWidth, maxHeight);

        // Superposition de chaque image dans l'ordre (ordre des paramètres)
        foreach (var image in images)
        {
            // Calcul du centrage de l'image sur le canvas final
            var offsetX = (maxWidth - image.Width) / 2;
            var offsetY = (maxHeight - image.Height) / 2;

            // Composition de l'image avec alpha blending
            result.Mutate(ctx => ctx.DrawImage(image, new Point(offsetX, offsetY), 1f));
        }

        return CropToSquareWithContent(result);
    }

    public static string CreatePreviewFrom(params SpriteFile?[] files)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
        
        var images = new List<Image<Rgba32>>();

        foreach (var file in files)
        {
            if (file == null) continue;
            if (file.Sprite.Image == null)
            {
                file.Sprite.Load();
            }
            images.Add(file.Sprite.Image!);
        }

        var preview = CreatePreviewImageFrom(images.ToArray());
        preview.Save(filePath);
        preview.Dispose();
        
        Console.WriteLine($"Created preview image: {filePath}");

        return filePath;
    }
}