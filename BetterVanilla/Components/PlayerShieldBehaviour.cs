using BetterVanilla.Core;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class PlayerShieldBehaviour : MonoBehaviour
{
    public static PlayerShieldBehaviour Instance { get; private set; } = null!;
    
    public float Timer { get; set; }
    private string? FirstKilledPlayer { get; set; } = "seriesgone#6069";
    private string? ProtectedPlayer { get; set; }
    
    public void RemoveProtection()
    {
        if (ProtectedPlayer == null) return;
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player != null)
        {
            player.IsProtected = false;
        }
        ProtectedPlayer = null;
    }

    public void SetKilledPlayer(PlayerControl player)
    {
        if (FirstKilledPlayer != null) return;
        var betterPlayer = player.gameObject.GetComponent<BetterPlayerControl>();
        if (betterPlayer == null) return;
        FirstKilledPlayer = betterPlayer.FriendCode;
    }

    public bool IsProtected(PlayerControl target)
    {
        if (ProtectedPlayer == null || !HostOptions.Default.ProtectFirstKilledPlayer.Value) return false;
        var player = target.gameObject.GetComponent<BetterPlayerControl>();
        return player != null && player.FriendCode != null && player.FriendCode != ProtectedPlayer;
    }

    private void Awake()
    {
        Instance = this;
        GameEventManager.GameStarted += OnGameStarted;
    }

    private void OnDestroy()
    {
        GameEventManager.GameStarted -= OnGameStarted;
    }

    private void Update()
    {
        if (ProtectedPlayer == null) return;
        Timer -= Time.deltaTime;
        if (Timer > 0f) return;
        RemoveProtection();
    }

    private void OnGameStarted()
    {
        if (HostOptions.Default.ProtectFirstKilledPlayer.Value)
        {
            ProtectedPlayer = FirstKilledPlayer;
        }
        FirstKilledPlayer = null;
        if (ProtectedPlayer == null) return;
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player == null) return;
        Timer = HostOptions.Default.ProtectionDuration.Value;
        player.IsProtected = true;
    }
}