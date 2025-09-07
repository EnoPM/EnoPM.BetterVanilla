using System.Reflection;
using BetterVanilla.Installer.Utils;

namespace BetterVanilla.Installer;

internal static class Program
{
    private static async Task Main()
    {
        PrintLogo();
        try
        {
            var gameDirectoryPath = Directory.GetCurrentDirectory();

            ProcessUtility.EnsureGameDirectoryIsValidAsync(gameDirectoryPath);
            ProcessUtility.EnsureProcessIsNotRunning(gameDirectoryPath);

            var installer = new ModInstaller(gameDirectoryPath);
            await installer.StartAsync();

            Console.WriteLine();
            ConsoleUtility.WriteLine(ConsoleColor.Green, "[X] BetterVanilla was successfully installed!");
        }
        catch (Exception ex)
        {
            ConsoleUtility.WriteLine(ConsoleColor.DarkRed, ex.Message);
        }
        Console.WriteLine();
        ConsoleUtility.WriteLine(ConsoleColor.DarkMagenta, "Press any key to exit...");
        Console.ReadKey(true);
    }

    private static void PrintLogo()
    {
        var logoFile = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("BetterVanilla.Installer.logo.txt");
        if (logoFile == null) return;
        var logoText = new StreamReader(logoFile).ReadToEnd();
        var rows = logoText.Split('\n');
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            ConsoleUtility.WriteLine(ConsoleColor.Yellow, row);
        }
        Console.WriteLine();
    }
}