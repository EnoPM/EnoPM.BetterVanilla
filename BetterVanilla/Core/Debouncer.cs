using System;
using System.Threading;
using System.Threading.Tasks;

namespace BetterVanilla.Core;

public sealed class Debouncer
{
    public event Action? Debounced;
    
    private CancellationTokenSource? TokenSource { get; set; }
    public TimeSpan Delay { get; set; }

    public Debouncer(TimeSpan delay)
    {
        Delay = delay;
    }

    public void Trigger()
    {
        TokenSource?.Cancel();
        TokenSource = new CancellationTokenSource();
        var token = TokenSource.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Delay, token);
                if (token.IsCancellationRequested) return;
                Debounced?.Invoke();
            }
            catch (TaskCanceledException)
            {
                // ignored - debounce canceled
            }
        }, token);
    }
}