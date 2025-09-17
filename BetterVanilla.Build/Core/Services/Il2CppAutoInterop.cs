using BetterVanilla.Build.Core.Arguments;

namespace BetterVanilla.Build.Core.Services;

public sealed class Il2CppAutoInterop : BaseService
{
    public StorageService Storage { get; }
    
    public Il2CppAutoInterop(StorageService storage, CompileArgumentOptions options) : base(options)
    {
        Storage = storage;
    }
}