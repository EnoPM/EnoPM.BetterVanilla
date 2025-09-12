using BetterVanilla.Core;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class PlayerShieldBehaviour : MonoBehaviour
{
    public static PlayerShieldBehaviour Instance { get; private set; } = null!;
    
    public float Timer { get; set; }
    private string? FirstKilledPlayer { get; set; }
    public string? ProtectedPlayer { get; private set; }
    private string? ProtectedPlayerName { get; set; }
    private int PlayerNameSetTimer { get; set; }
    
    public void RemoveProtection()
    {
        if (ProtectedPlayer == null) return;
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player?.Player != null)
        {
            player.IsProtected = false;
            player.Player.RpcSetName(ProtectedPlayerName);
            Ls.LogInfo($"Removed protection for {ProtectedPlayerName}");
        }
        ProtectedPlayer = null;
        ProtectedPlayerName = null;
        PlayerNameSetTimer = 0;
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
        if (ProtectedPlayer == null || AmongUsClient.Instance == null || !AmongUsClient.Instance.IsGameStarted) return;
        Timer -= Time.deltaTime;
        var playerNameSetTimer = Mathf.FloorToInt(Timer);
        if (playerNameSetTimer != PlayerNameSetTimer)
        {
            PlayerNameSetTimer = playerNameSetTimer;
            UpdatePlayerName();
        }
        if (Timer > 0f) return;
        RemoveProtection();
    }

    private void UpdatePlayerName()
    {
        if (ProtectedPlayer == null) return;
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player != null && player.Player != null)
        {
            //player.Player.RpcSetName($"{ProtectedPlayerName}\n<size=50%>Protected ({Mathf.RoundToInt(Timer)}s)</size>");
            player.Player.RpcSetName(@$"{ProtectedPlayerName} <size=50%>({PlayerNameSetTimer}s)</size>");
        }
    }

    private void OnGameStarted()
    {
        if (!HostOptions.Default.ProtectFirstKilledPlayer.Value)
        {
            Ls.LogInfo($"Protection is disabled by host");
            return;
        }
        if (!LocalConditions.AmHost())
        {
            Ls.LogInfo($"Protection is only allowed for host");
            return;
        }
        
        if (FirstKilledPlayer != null)
        {
            ProtectedPlayer = FirstKilledPlayer;
        }
        FirstKilledPlayer = null;
        if (ProtectedPlayer == null)
        {
            Ls.LogInfo($"No player to protect");
            return;
        }
        
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player?.Player == null)
        {
            Ls.LogInfo($"Unable to find player by friend code: {ProtectedPlayer}");
            return;
        }
        
        Timer = HostOptions.Default.ProtectionDuration.Value;
        player.IsProtected = true;
        ProtectedPlayerName = player.Player.Data.PlayerName;
        PlayerNameSetTimer = 0;
        
        Ls.LogInfo($"Protection enabled for {player.Player.Data?.PlayerName} during {Timer}s");
    }
}