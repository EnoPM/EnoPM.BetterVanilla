using BetterVanilla.CosmeticsCompiler.Commands;

namespace BetterVanilla.CosmeticsCompiler;

public static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "cosmetics-bundle-config.txt");
            if (File.Exists(path))
            {
                RunFromFile(path);
                return 0;
            }
        }
        if (args.Length == 1 && args[0].StartsWith('@'))
        {
            var path = args[0][1..].Trim('"');
            RunFromFile(path);
            return 0;
        }
        return Run(args);
    }

    private static void RunFromFile(string path)
    {
        Console.WriteLine($"Running from file {path}");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }

        var lines = File.ReadAllLines(path);
        var current = new List<string>();
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith('#')) continue;

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                RunIfNotEmpty(current);
                current.Clear();
            }
            else
            {
                if (trimmedLine.StartsWith('"') && trimmedLine.EndsWith('"'))
                {
                    // Ligne avec guillemets - c'est un argument, enlever les guillemets
                    current.Add(trimmedLine.Substring(1, trimmedLine.Length - 2));
                }
                else
                {
                    // Ligne sans guillemets - c'est probablement une commande ou un flag
                    current.Add(trimmedLine);
                }
            }
        }
        RunIfNotEmpty(current);
    }
    
    private static void RunIfNotEmpty(List<string> args)
    {
        if (args.Count <= 0) return;
        Run(args.ToArray());
    }

    private static int Run(string[] args)
    {
        var root = new Root();
        
        root.AddSubcommand(new BundleCommand());
        root.AddSubcommand(new CreateHatCommand());
        root.AddSubcommand(new CreateVisorCommand());
        root.AddSubcommand(new CreateNameplateCommand());
        
        root.Execute(args);

        return 0;

        //var rootCommand = Commands.CreateRootCommand();
        //return rootCommand.Parse(args).Invoke();
    }

}