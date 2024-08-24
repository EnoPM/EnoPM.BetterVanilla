namespace AmongUsDevKit.Utils;

internal sealed class ArgumentsReader
{
    private readonly List<string> _arguments = [];
    private readonly List<string> _flags = [];
    public string[] RemainingArguments => _arguments.ToArray();
    public int Size => _arguments.Count;

    #region Flags

    public bool HasMemoryFlag => HasFlag("memory", 'm');
    public bool HasDeferredFlag => HasFlag("deferred", 'd');
    public bool HasVerboseFlag => HasFlag("verbose", 'v');
    public bool HasVersionRandomizerFlag => HasFlag("randomizer", 'r');
    public bool HasUnityProjectFlag => HasFlag("unity", 'u');

    #endregion


    public void Init(string[] args)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith('-'))
            {
                var flag = arg[(arg.StartsWith("--") ? "--" : "-").Length..];
                _flags.Add(flag);
            }
            else
            {
                _arguments.Add(arg);
            }
        }
    }

    public string Consume()
    {
        var result = _arguments.First();
        _arguments.RemoveAt(0);
        return result;
    }

    private bool HasFlag(string flag) => _flags.Contains(flag);
    private bool HasFlag(string flag, char shortFlag) => HasFlag(flag) || HasFlag(shortFlag.ToString());
}