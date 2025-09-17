using System.Diagnostics;
using BetterVanilla.Build.Core.Arguments;

namespace BetterVanilla.Build.Core.Services;

public sealed class DotNetBuilder : BaseService
{
    
    public DotNetBuilder(CompileArgumentOptions options) : base(options)
    {
        
    }
    
    public async Task BuildWindowsProject()
    {
        
    }

    private async Task StartProcess()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = ""
        };
    }
}