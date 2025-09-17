using BetterVanilla.Build.Core;
using BetterVanilla.Build.Core.Arguments;
using BetterVanilla.ToolsLib.Utils;
using CommandLine;

namespace BetterVanilla.Build;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var parseResult = Parser.Default.ParseArguments<CompileArgumentOptions>(args);

        if (parseResult.Tag == ParserResultType.Parsed)
        {
            var parsed = (Parsed<CompileArgumentOptions>)parseResult;
            return await CompileAsync(parsed.Value);
        }
        var notParsed = (NotParsed<CompileArgumentOptions>)parseResult;
        return HandleError(notParsed.Errors);
    }

    private static async Task<int> CompileAsync(CompileArgumentOptions argumentOptions)
    {
        return 0;
    }
    
    private static int HandleError(IEnumerable<Error> errors)
    {
        ConsoleUtility.WriteLine(ConsoleColor.Red, $"{errors.Count()} parsing errors occurred");
        return 1;
    }

    private static async Task<int> RunBuild(BuildOptions options)
    {
        try
        {
            var config = new BuildConfiguration(options);
            var builder = new PluginBuilder(config);

            await builder.BuildAsync();

            Console.WriteLine("[+] Project built successfully");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Error: {ex.Message}");
            if (options.Verbose)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return 1;
        }
    }
}