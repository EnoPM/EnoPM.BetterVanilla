using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;

namespace BetterVanilla.Core;

public static class Ls
{
    private static ManualLogSource Logger { get; set; }

    public static void SetLogSource(ManualLogSource logSource)
    {
        Logger = logSource;
    }

    public static void LogError(object data) => Logger.LogError(data);
    public static void LogError(BepInExErrorLogInterpolatedStringHandler logHandler) => Logger.LogError(logHandler);
    
    public static void LogWarning(object data) => Logger.LogWarning(data);
    public static void LogWarning(BepInExErrorLogInterpolatedStringHandler logHandler) => Logger.LogWarning(logHandler);
    
    public static void LogMessage(object data) => Logger.LogMessage(data);
    public static void LogMessage(BepInExErrorLogInterpolatedStringHandler logHandler) => Logger.LogMessage(logHandler);
    
    public static void LogInfo(object data) => Logger.LogInfo(data);
    public static void LogInfo(BepInExErrorLogInterpolatedStringHandler logHandler) => Logger.LogInfo(logHandler);
    
    public static void LogDebug(object data) => Logger.LogDebug(data);
    public static void LogDebug(BepInExErrorLogInterpolatedStringHandler logHandler) => Logger.LogDebug(logHandler);
}