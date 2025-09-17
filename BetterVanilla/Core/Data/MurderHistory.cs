using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Components;

namespace BetterVanilla.Core.Data;

public sealed class MurderHistory
{
    private static List<MurderHistory> CurrentGame { get; }
    private static void OnGameStarted() => CurrentGame.Clear();

    public static void RegisterKill(PlayerControl murder, PlayerControl victim)
    {
        CurrentGame.Add(new MurderHistory(murder, victim));
    }

    public static bool CanKill(PlayerControl murder, PlayerControl _)
    {
        var murderPc = murder.gameObject.GetComponent<BetterPlayerControl>();
        if (murderPc == null) return true;
        var friendCode = murder.FriendCode;
        if (friendCode == null) return true;
        var allowedTime = DateTime.UtcNow.AddSeconds(-2);
        return CurrentGame.All(x => x.CreatedAt <= allowedTime);
    }

    static MurderHistory()
    {
        CurrentGame = [];
        GameEventManager.GameStarted += OnGameStarted;
    }
    
    public DateTime CreatedAt { get; }
    public string Murder { get; }
    public string Target { get; }

    private MurderHistory(PlayerControl murder, PlayerControl target)
    {
        CreatedAt = DateTime.UtcNow;

        var murderPc = murder.gameObject.GetComponent<BetterPlayerControl>();
        var targetPc = target.gameObject.GetComponent<BetterPlayerControl>();
        
        Murder = murderPc?.FriendCode ?? string.Empty;
        Target = targetPc?.FriendCode ?? string.Empty;
    }
}