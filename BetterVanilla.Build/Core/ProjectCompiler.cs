using BetterVanilla.Build.Core.Arguments;
using BetterVanilla.Build.Core.Services;

namespace BetterVanilla.Build.Core;

public sealed class ProjectCompiler : BaseService, IDisposable
{
    private StorageService Storage { get; }
    private string ProjectDirectory { get; }

    public ProjectCompiler(CompileArgumentOptions options) : base(options)
    {
        ProjectDirectory = Path.GetDirectoryName(Path.GetFullPath(Options.CsprojFileInputPath))
                           ?? throw new DirectoryNotFoundException($"Project directory could not be found: {Options.CsprojFileInputPath}");
        Storage = new StorageService();
    }

    public async Task CompileAsync()
    {
        
    }

    public void Dispose()
    {
        Storage.Dispose();
    }
}