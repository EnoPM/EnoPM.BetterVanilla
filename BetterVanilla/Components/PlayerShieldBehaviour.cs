using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class PlayerShieldBehaviour : MonoBehaviour
{
    public static PlayerShieldBehaviour Instance { get; private set; } = null!;
    
    private float Timer { get; set; }
    private string? FirstKilledPlayer { get; set; }
    private string? ProtectedPlayer { get; set; }
    private string? ProtectedPlayerName { get; set; }
    
    private void RemoveProtection(bool rpc = true)
    {
        if (ProtectedPlayer == null) return;
        var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
        if (player?.Player != null)
        {
            if (rpc)
            {
                player.Player.RpcSetName(ProtectedPlayerName);
            }
            Ls.LogInfo($"Removed protection for {ProtectedPlayerName}");
        }
        ProtectedPlayer = null;
        ProtectedPlayerName = null;
    }

    public void SetKilledPlayer(PlayerControl player)
    {
        if (!LocalConditions.AmHost())
        {
            Ls.LogInfo($"{nameof(FirstKilledPlayer)} can only be set by the host");
            return;
        }
        if (!string.IsNullOrEmpty(FirstKilledPlayer))
        {
            Ls.LogWarning($"{nameof(FirstKilledPlayer)} is already set to {FirstKilledPlayer}");
            return;
        }
        var betterPlayer = player.gameObject.GetComponent<BetterPlayerControl>();
        if (betterPlayer == null)
        {
            Ls.LogMessage($"No {nameof(BetterPlayerControl)} found for player {player.Data.PlayerName}");
            return;
        }
        if (BetterPlayerControl.LocalPlayer == null)
        {
            Ls.LogWarning($"No local player found to send the rpc");
            return;
        }
        if (betterPlayer.FriendCode == null)
        {
            Ls.LogWarning($"{nameof(BetterPlayerControl)} {betterPlayer.Player?.Data.PlayerName} has no friend code");
            return;
        }
        BetterPlayerControl.LocalPlayer.RpcSetFirstKilledPlayer(betterPlayer.FriendCode);
    }

    public void SetFirstKilledPlayer(string? friendCode)
    {
        Ls.LogMessage($"Set first killed player {friendCode}");
        FirstKilledPlayer = friendCode;
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
        GameEventManager.GameReallyStarted += OnGameStarted;
        GameEventManager.GameEnded += OnGameEnded;
    }

    private void OnDestroy()
    {
        GameEventManager.GameReallyStarted -= OnGameStarted;
        GameEventManager.GameEnded -= OnGameEnded;
    }

    private void Update()
    {
        if (ProtectedPlayer == null || !LocalConditions.IsGameStarted()) return;
        Timer -= Time.deltaTime;
        if (Timer > 0f) return;
        RemoveProtection();
    }

    private void OnGameEnded()
    {
        RemoveProtection(false);
        StopAllCoroutines();
    }

    private IEnumerator CoInitializeProtection()
    {
        if (!HostOptions.Default.ProtectFirstKilledPlayer.Value)
        {
            Ls.LogInfo($"Protection is disabled by host");
            yield break;
        }
        if (!LocalConditions.AmHost())
        {
            Ls.LogInfo($"Protection is only allowed for host");
            yield break;
        }

        var firstKilledPlayer = FirstKilledPlayer;
        SetFirstKilledPlayer(null);

        yield return new WaitForEndOfFrame();

        if (!string.IsNullOrEmpty(firstKilledPlayer))
        {
            ProtectedPlayer = firstKilledPlayer;
        }

        if (!string.IsNullOrEmpty(ProtectedPlayer))
        {
            var player = BetterVanillaManager.Instance.GetPlayerByFriendCode(ProtectedPlayer);
            if (player == null || player.Player == null)
            {
                Ls.LogInfo($"Unable to find player by friend code: {ProtectedPlayer}");
                yield break;
            }
            
            Timer = HostOptions.Default.ProtectionDuration.Value;
            ProtectedPlayerName = player.Player.Data.PlayerName;
            yield return new WaitForSeconds(5f); // Wait 5s before sending RPC
            player.Player.RpcSetName(ColorUtils.ColoredString(Palette.CrewmateBlue, ProtectedPlayerName));
            
            Ls.LogInfo($"Protection enabled for {ProtectedPlayerName} during {Timer}s");
        }
        
        yield return new WaitForEndOfFrame();

        if (!string.IsNullOrEmpty(FirstKilledPlayer))
        {
            FirstKilledPlayer = null;
        }
    }

    private void OnGameStarted() => this.StartCoroutine(CoInitializeProtection());
}