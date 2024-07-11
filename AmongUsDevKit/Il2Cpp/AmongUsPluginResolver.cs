using Mono.Cecil;

namespace AmongUsDevKit.Il2Cpp;

public sealed class AmongUsPluginResolver : DefaultAssemblyResolver
{
    private readonly Dictionary<string, AssemblyDefinition?> _cache = [];
    public List<AssemblyDefinition> AllAssemblyDefinitions => _cache.Where(x => x.Value != null).Select(x => x.Value!).ToList();

    public void RegisterDirectory(string directoryPath)
    {
        var directory = new DirectoryInfo(directoryPath);
        foreach (var file in directory.GetFiles("*.dll"))
        {
            RegisterFile(file.FullName);
        }
    }

    public void LoadLibraries(AssemblyDefinition mainAssembly)
    {
        foreach (var item in _cache)
        {
            try
            {
                var assembly = AssemblyDefinition.ReadAssembly(item.Key, new ReaderParameters
                {
                    AssemblyResolver = this,
                    ReadingMode = Program.Arguments.HasDeferredFlag ? ReadingMode.Deferred : ReadingMode.Immediate,
                    ReadWrite = false,
                    InMemory = Program.Arguments.HasMemoryFlag
                });
                _cache[item.Key] = assembly;
                var existingReference = mainAssembly.MainModule.AssemblyReferences.FirstOrDefault(x => x.Name == assembly.Name.Name && x.Version == assembly.Name.Version);
                if (existingReference != null && existingReference.PublicKeyToken.Length != assembly.Name.PublicKeyToken.Length)
                {
                    Log.Verbose($"Found reference {existingReference.FullName} in {mainAssembly.Name.Name} but trying to add {assembly.Name.FullName}", ConsoleColor.Yellow);
                    continue;
                }
                mainAssembly.MainModule.AssemblyReferences.Add(assembly.Name);
                Log.Verbose($"Successfully added reference {assembly.Name.Name} in {mainAssembly.Name.Name}", ConsoleColor.Green);
            }
            catch(Exception exception)
            {
                if (exception is BadImageFormatException) continue;
                Log.Production($"[{nameof(AmongUsPluginResolver)}:{nameof(LoadLibraries)}] Unable to load library {item.Key}: {exception.GetType().FullName}", ConsoleColor.Red);
            }
        }
    }

    public override AssemblyDefinition? Resolve(AssemblyNameReference name) => ResolveAssemblyDefinition(name);
    public TypeDefinition? ResolveTypeInReferences(string fullName)
    {
        foreach (var assembly in AllAssemblyDefinitions)
        {
            var result = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
    
    private AssemblyDefinition? ResolveAssemblyDefinition(AssemblyNameReference name) => AllAssemblyDefinitions.FirstOrDefault(x => x.Name.Name == name.Name && x.Name.Version == name.Version);

    private void RegisterFile(string filePath)
    {
        _cache[filePath] = null;
    }
}