using System.Text.Json;

namespace AmongUsDevKit.Utils;

internal static class Log
{
    private static readonly PersonalLogger ProductionLogger = new();
    private static readonly PersonalLogger VerboseLogger = new(rawPrefix: "*");
    
    public static void Production(string message) => ProductionLogger.Log(message);

    public static void Production(string message, ConsoleColor color) => ProductionLogger.Log(message, color);

    public static void Production(string message, ConsoleColor color, ConsoleColor background) => ProductionLogger.Log(message, color, background);

    public static void Verbose(string message)
    {
        if (!Program.Arguments.HasVerboseFlag) return;
        VerboseLogger.Log(message);
    }

    public static void Verbose(string message, ConsoleColor color)
    {
        if (!Program.Arguments.HasVerboseFlag) return;
        VerboseLogger.Log(message, color);
    }

    public static void Verbose(string message, ConsoleColor color, ConsoleColor background)
    {
        if (!Program.Arguments.HasVerboseFlag) return;
        VerboseLogger.Log(message, color, background);
    }

    public static void ProductionS(object message) => Production(Serialize(message));
    public static void ProductionS(object message, ConsoleColor color) => Production(Serialize(message), color);
    public static void ProductionS(object message, ConsoleColor color, ConsoleColor background) => Production(Serialize(message), color, background);

    public static void VerboseS(object message) => Verbose(Serialize(message));
    public static void VerboseS(object message, ConsoleColor color) => Verbose(Serialize(message), color);
    public static void VerboseS(object message, ConsoleColor color, ConsoleColor background) => Verbose(Serialize(message), color, background);

    private static string Serialize(object message) => JsonSerializer.Serialize(message);

    public static PersonalLogger CreateLogger(string? prefix = null) => new(prefix);
    
    internal sealed class PersonalLogger(string? prefix = null, string rawPrefix = "")
    {
        private string HandlePrefix(string message) => prefix == null ? $"{rawPrefix}{message}" : $"{rawPrefix}[{prefix}] {message}";

        public void Log(string message)
        {
            Console.WriteLine(HandlePrefix(message));
        }

        public void Log(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(HandlePrefix(message));
            Console.ResetColor();
        }

        public void Log(string message, ConsoleColor color, ConsoleColor background)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = background;
            Console.WriteLine(HandlePrefix(message));
            Console.ResetColor();
        }
    }
}