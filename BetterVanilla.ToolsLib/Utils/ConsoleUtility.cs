namespace BetterVanilla.ToolsLib.Utils;

public static class ConsoleUtility
{
    private const int ProgressBarWidth = 50;
    private const char ProgressFilledChar = '#';
    private const char ProgressEmptyChar = '-';
    
    public static void WriteConsoleProgress(double progress)
    {
        if (progress < 0)
        {
            progress = 0;
        }
        if (progress > 1)
        {
            progress = 1;
        }

        var filled = (int)(progress * ProgressBarWidth);

        var bar = new string(ProgressFilledChar, filled) + new string(ProgressEmptyChar, ProgressBarWidth - filled);
        var percent = (progress * 100).ToString("0.0").PadLeft(5);

        var previousColor = Console.ForegroundColor;
        
        Console.ForegroundColor = progress < 1 ? ConsoleColor.DarkYellow : ConsoleColor.DarkGreen;
        Console.Write($"\r[{bar}] {percent}%");
        
        Console.ForegroundColor = previousColor;
    }

    public static void WriteLine(ConsoleColor color, string message)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previousColor;
    }
    
    public static void NewLine() => Console.WriteLine();
}