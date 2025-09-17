using BetterVanilla.Build.Core.Interfaces;
using CommandLine;
using JetBrains.Annotations;

namespace BetterVanilla.Build.Core.Arguments;

[Verb("compile", true, HelpText = "Compile Among Us plugin using Il2CppAutoInterop"), UsedImplicitly]
public sealed class CompileArgumentOptions : BaseArgumentOptions, IBepInExConfig, IIl2CppAutoInteropConfig
{
    [Option('i', "input", Required = true, HelpText = ".csproj input file path")]
    public string CsprojFileInputPath { get; set; } = null!;
    
    [Option('o', "output", Required = true, HelpText = "Output file path")]
    public string OutputFilePath { get; set; } = null!;
    
    [Option("AmongUs-interop-url", HelpText = "Among Us interop archive url", Required = true)]
    public string AmongUsInteropUrl { get; set; } = null!;
    
    [Option("android-output", HelpText = "Android output file path")]
    public string? AndroidOutputFilePath { get; set; }

    [Option("auto-interop-version", HelpText = "Il2CppAutoInterop version", Default = "1.1.0")]
    public string Il2CppAutoInteropVersion { get; set; } = "1.1.0";
    
    [Option("auto-interop-runtime", HelpText = "Il2CppAutoInterop runtime", Default = "win-x64")]
    public string Il2CppAutoInteropRuntime { get; set; } = "win-x64";
    
    [Option("bepinex-version", HelpText = "BepInEx version", Default = "6.0.0")]
    public string BepInExVersion { get; set; } = "6.0.0";

    [Option("bepinex-build-number", HelpText = "BepInEx build number", Default = 738)]
    public uint BepInExBuildNumber { get; set; } = 738;
    
    [Option("bepinex-build-hash", HelpText = "BepInEx build hash", Default = "af0cba7")]
    public string BepInExBuildHash { get; set; } = "af0cba7";
    
    [Option('u', "unity-project", HelpText = "Unity project directory")]
    public string? UnityProjectDirectory { get; set; }
}