using BetterVanilla.Build.Core.Interfaces;
using CommandLine;

namespace BetterVanilla.Build.Core.Arguments;

public abstract class BaseArgumentOptions : IConsoleConfig
{
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Enable verbose logging")]
    public bool Verbose { get; set; } = false;
}