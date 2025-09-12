namespace BetterVanilla.Cosmetics.Core;

public static class Github
{
    public const string Repository = "EnoPM/EnoPM.BetterVanilla";
    public const string MainBranch = "master";

    public static string GetFileUrl(string file) => $"https://raw.githubusercontent.com/{Repository}/refs/heads/{MainBranch}/{file}";
}