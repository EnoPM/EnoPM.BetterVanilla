using BetterVanilla.Build.Core.Arguments;

namespace BetterVanilla.Build.Core.Services;

public abstract class BaseService
{
    protected CompileArgumentOptions Options { get; }
    
    protected BaseService(CompileArgumentOptions options)
    {
        Options = options;
    }
}