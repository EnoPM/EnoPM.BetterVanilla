namespace BetterVanilla.ToolsLib.Utils;

public static class DoorstopUtility
{
    public const string EntryPointFilename = "winhttp.dll";
    public const string DoorstopConfigFilename = "doorstop_config.ini";
    
    private const string TargetAssemblyEntry = "target_assembly";
    private const string CoreClrPathEntry = "coreclr_path";
    private const string CorLibDirEntry = "corlib_dir";

    public static void UpdateDoorstopConfigFile(string doorstopConfigFilePath, string bepInExDirectory)
    {
        var content = File.ReadAllText(doorstopConfigFilePath);
        var rows = content.Split('\n');
        for (var i = 0; i < rows.Length; i++)
        {
            if (rows[i].StartsWith($"{TargetAssemblyEntry} ="))
            {
                rows[i] = $"{TargetAssemblyEntry} = {Path.Combine(bepInExDirectory, "BepInEx", "core", "BepInEx.Unity.IL2CPP.dll")}";
                continue;
            }
            if (rows[i].StartsWith($"{CoreClrPathEntry} ="))
            {
                rows[i] = $"{CoreClrPathEntry} = {Path.Combine(bepInExDirectory, "dotnet", "coreclr.dll")}";
                continue;
            }
            if (rows[i].StartsWith($"{CorLibDirEntry} ="))
            {
                rows[i] = $"{CorLibDirEntry} = {Path.Combine(bepInExDirectory, "dotnet")}";
            }
        }
        File.WriteAllText(doorstopConfigFilePath, string.Join('\n', rows));
    }
}