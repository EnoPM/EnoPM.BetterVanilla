namespace BetterVanilla.Cosmetics.Api.Core;

public interface IResourceFile
{
    public string Name { get; set; }
    public string Hash { get; set; }
    public string DownloadUrl { get; set; }
}