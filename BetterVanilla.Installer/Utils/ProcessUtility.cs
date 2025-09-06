using System.Diagnostics;

namespace BetterVanilla.Installer.Utils;

public static class ProcessUtility
{
    public static void EnsureGameDirectoryIsValidAsync(string gameDirectoryPath)
    {
        if (File.Exists(GetProcessExecutablePath(gameDirectoryPath))) return;
        throw new Exception("BetterVanillaInstaller must be executed in Among Us game directory");
    }
    
    public static void EnsureProcessIsNotRunning(string gameDirectoryPath)
    {
        var processPath = GetProcessExecutablePath(gameDirectoryPath);
        var processes = GetProcessesByPath(processPath).ToArray();
        if (processes.Length == 0) return;

        foreach (var process in processes)
        {
            ConsoleUtility.WriteLine(ConsoleColor.Red, $"Closing process: {process.ProcessName}");
            try
            {
                if (!process.CloseMainWindow())
                {
                    process.Kill();
                }

                if (!process.WaitForExit(10_000))
                {
                    process.Kill(true);
                    process.WaitForExit(10_000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to close process {process.ProcessName}", ex);
            }
        }
        if (!IsProcessRunning(processPath)) return;
        throw new Exception("BetterVanillaInstaller must be started when the game is closed");
        
    }

    private static bool IsProcessRunning(string processPath)
    {
        return GetProcessesByPath(processPath).Any();
    }
    
    private static IEnumerable<Process> GetProcessesByPath(string processPath)
    {
        foreach (var process in Process.GetProcesses())
        {
            try
            {
                if (!string.Equals(process.MainModule?.FileName, processPath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }
            catch
            {
                // ignored
                continue;
            }
            yield return process;
        }
    }

    private static string GetProcessExecutablePath(string gameDirectoryPath)
    {
        return Path.Combine(gameDirectoryPath, "Among Us.exe");
    }
}