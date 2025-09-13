using System;

namespace BetterVanilla.Core;

public static class GameEventManager
{
    public static event Action? MeetingStarted;
    public static event Action<PlayerControl>? MeetingEnded;
    public static event Action? GameStarted;
    public static event Action? GameReallyStarted;
    public static event Action? GameEnded;
    public static event Action<PlayerControl>? PlayerJoined; 
    public static event Action<PlayerControl>? PlayerReady; 

    public static void TriggerMeetingStarted() => MeetingStarted?.Invoke();
    public static void TriggerMeetingEnded(PlayerControl exiledPlayer) => MeetingEnded?.Invoke(exiledPlayer);
    public static void TriggerGameStarted() => GameStarted?.Invoke();
    public static void TriggerGameReallyStarted() => GameReallyStarted?.Invoke();
    public static void TriggerGameEnded() => GameEnded?.Invoke();
    public static void TriggerPlayerJoined(PlayerControl player) => PlayerJoined?.Invoke(player);
    public static void TriggerPlayerReady(PlayerControl player) => PlayerReady?.Invoke(player);
}