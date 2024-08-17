global using AmongUsDevKit.Utils;

using AmongUsDevKit.Il2Cpp;

namespace AmongUsDevKit;

internal static class Program
{
    public static readonly ArgumentsReader Arguments = new();
    
    private static void Main(string[] args)
    {
        Arguments.Init(args);
        var command = Arguments.Consume().ToLowerInvariant();
        switch (command)
        {
            case "enhance":
                Enhance(Arguments);
                break;
            default:
                throw new Exception($"Unknown command {command}");
        }
    }

    private static void Enhance(ArgumentsReader args)
    {
        var input = args.Consume();
        var output = args.Consume();
        var libraryDirectories = args.RemainingArguments;

        var interopMaker = new InteropMaker(input);
        foreach (var directoryPath in libraryDirectories)
        {
            interopMaker.Resolver.RegisterDirectory(directoryPath);
        }
        interopMaker.LoadDependencies();
        interopMaker.RemoveAssemblyNameSuffix(".Mono");
        interopMaker.RandomizeAssemblyVersion();
        interopMaker.RegisterPluginEntryPoint();
        
        var monoBehaviourInterop = new MonoBehaviourInterop(interopMaker);
        monoBehaviourInterop.Run();
        
        interopMaker.Save(output);
    }
}