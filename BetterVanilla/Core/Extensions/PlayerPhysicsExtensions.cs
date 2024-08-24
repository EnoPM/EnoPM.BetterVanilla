using System.Collections;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class PlayerPhysicsExtensions
{
    public static IEnumerator CoSpawnPlayer(this PlayerPhysics playerPhysics)
    {
        var lobby = LobbyBehaviour.Instance;
        if (!lobby)
        {
            yield break;
        }

        if (playerPhysics.myPlayer.AmOwner)
        {
            playerPhysics.inputHandler.enabled = true;
        }
        var spawnSeatId = playerPhysics.myPlayer.PlayerId % lobby.SpawnPositions.Length;
        var spawnPos = playerPhysics.Vec2ToPosition(lobby.SpawnPositions[spawnSeatId]);
        playerPhysics.myPlayer.cosmetics.ToggleName(false);
        playerPhysics.myPlayer.Collider.enabled = false;
        playerPhysics.myPlayer.NetTransform.enabled = false;
        KillAnimation.SetMovement(playerPhysics.myPlayer, false);
        yield return new WaitForFixedUpdate();
        playerPhysics.myPlayer.cosmetics.SetForcedVisible(true);
        var amFlipped = spawnSeatId > 4;
        SoundManager.Instance.PlaySound(lobby.SpawnSound, false).volume = 0.75f;
        playerPhysics.FlipX = amFlipped;
        playerPhysics.myPlayer.cosmetics.AnimateSkinSpawn();
        playerPhysics.myPlayer.transform.position = spawnPos;
        yield return playerPhysics.Animations.CoPlaySpawnAnimation(playerPhysics.FlipX);
        playerPhysics.transform.position = spawnPos + new Vector3(amFlipped ? -0.3f : 0.3f, -0.24f);
        playerPhysics.ResetMoveState(false);
        Vector2 normalized = (-spawnPos).normalized;
        yield return playerPhysics.WalkPlayerTo((Vector2) spawnPos + normalized);
        playerPhysics.myPlayer.Collider.enabled = true;
        KillAnimation.SetMovement(playerPhysics.myPlayer, true);
        playerPhysics.myPlayer.NetTransform.enabled = true;
        playerPhysics.myPlayer.NetTransform.ClearPositionQueues();
        playerPhysics.myPlayer.cosmetics.ToggleName(true);
        if (playerPhysics.myPlayer.AmOwner)
        {
            playerPhysics.inputHandler.enabled = false;
        }
        
        GameEventManager.TriggerPlayerReady(playerPhysics.myPlayer);
    }
}