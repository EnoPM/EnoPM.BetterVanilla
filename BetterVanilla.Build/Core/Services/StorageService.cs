namespace BetterVanilla.Build.Core.Services;

public sealed class StorageService : IDisposable
{
    private string MainDirectory { get; }
    private string TempDirectory { get; }
    private string CacheDirectory { get; }

    private static string RandomPathName => Guid.NewGuid().ToString();
    
    public StorageService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        
        MainDirectory = CreateDirectory(Path.Combine(appData, "BetterVanilla.Compiler"));
        TempDirectory = CreateDirectory(Path.Combine(MainDirectory, "Temp"));
        CacheDirectory = CreateDirectory(Path.Combine(MainDirectory, "Cache"));
    }

    public FileStream CreateTempFile()
    {
        var fileName = $"{RandomPathName}.tmp";
        var path = Path.Combine(TempDirectory, fileName);
        return File.Create(path);
    }

    public string CreateTempDirectory()
    {
        var path = Path.Combine(TempDirectory, RandomPathName);
        CreateDirectory(path);
        return path;
    }

    public string CreateCacheDirectory(string directoryName)
    {
        var path = Path.Combine(CacheDirectory, directoryName);
        CreateDirectory(path);
        return path;
    }

    private static string CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return path;
        Directory.CreateDirectory(path);
        return path;
    }
    
    public void Dispose()
    {
        if (Directory.Exists(TempDirectory))
        {
            Directory.Delete(TempDirectory, true);
        }
    }
}