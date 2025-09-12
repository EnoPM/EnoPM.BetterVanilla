using BetterVanilla.Core;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class PlayerShieldBehaviour : MonoBehaviour
{
    public static PlayerShieldBehaviour Instance { get; private set; } = null!;
    
    public float Timer { get; set; }
    private string? FirstKilledPlayer { get; set; } = "seriesgone#6069";
    public string? ProtectedPlayer { get; private set; }
    
    public void RemoveProtection()
    {
        if (ProtectedPlayer == null) return;
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player?.Player != null)
        {
            // Simple suppression de la protection côté BetterVanilla
            player.IsProtected = false;
            Ls.LogMessage($"Protection retirée pour {player.Player.Data?.PlayerName}");
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

    public bool IsPlayerProtected(PlayerControl target)
    {
        if (ProtectedPlayer == null || !HostOptions.Default.ProtectFirstKilledPlayer.Value) return false;
        var player = target.gameObject.GetComponent<BetterPlayerControl>();
        return player != null && player.FriendCode != null && player.FriendCode == ProtectedPlayer;
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
        if (!HostOptions.Default.ProtectFirstKilledPlayer.Value) return;
        if (!LocalConditions.AmHost()) return; // Seul l'hôte peut appliquer la protection
        
        if (FirstKilledPlayer != null)
        {
            ProtectedPlayer = FirstKilledPlayer;
        }
        FirstKilledPlayer = null;
        if (ProtectedPlayer == null) return;
        
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player?.Player == null) return;
        
        Timer = HostOptions.Default.ProtectionDuration.Value;
        player.IsProtected = true;
        
        // Avec DisableServerAuthority, pas besoin du système Guardian Angel
        // La protection est gérée directement dans CheckMurderPrefix
        
        Ls.LogMessage($"Protection activée pour {player.Player.Data?.PlayerName} pendant {Timer} secondes");
    }
}