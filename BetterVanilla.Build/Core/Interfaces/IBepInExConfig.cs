namespace BetterVanilla.Build.Core.Interfaces;

public interface IBepInExConfig
{
    public string BepInExVersion { get; }
    
    public uint BepInExBuildNumber { get; }
    
    public string BepInExBuildHash { get; }
    
    public string AmongUsInteropUrl { get; }
}