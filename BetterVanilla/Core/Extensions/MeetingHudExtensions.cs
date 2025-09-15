using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MeetingHudExtensions
{
    private static readonly List<VoteData> CachedVotes = [];
    private static bool PreviousDisplayColorState { get; set; }

    public static void BetterStart(this MeetingHud _)
    {
        CachedVotes.Clear();
    }

    public static void BetterCastVote(this MeetingHud _, byte voterId, byte votedId)
    {
        if (!LocalConditions.AmHost() || !HostOptions.Default.AllowDeadVoteDisplay.Value || BetterPlayerControl.LocalPlayer == null) return;
        var voter = BetterVanillaManager.Instance.GetPlayerById(voterId);
        if (!voter)
        {
            Ls.LogWarning($"Player {voterId} has not been found in BetterCastVote.");
            return;
        }
        BetterPlayerControl.LocalPlayer.RpcSetMeetingVote(voterId, votedId);
    }

    public static void CastVote(byte voterId, byte votedId)
    {
        var voter = BetterVanillaManager.Instance.GetPlayerById(voterId);
        var voted = BetterVanillaManager.Instance.GetPlayerById(votedId);

        if (voter == null)
        {
            Ls.LogError($"Unable to find voter by id {voterId}");
            return;
        }
        
        LogVote(voter, voted);

        if (CachedVotes.Any(x => x.Voter == voter))
        {
            Ls.LogError($"{voter.Player?.Data.PlayerName} vote is already cached");
            return;
        }

        CachedVotes.Add(new VoteData(voter, voted));
    }

    public static void BetterUpdate(this MeetingHud meetingHud)
    {
        if (meetingHud.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Proceeding)
        {
            DeleteAllCachedVoteSpreaders();
        }
        else
        {
            meetingHud.UpdateAllCachedVoteSpreaders();
        }
    }
    
    private static void DeleteAllCachedVoteSpreaders()
    {
        foreach (var vote in CachedVotes)
        {
            if(vote.Renderer == null) continue;
            vote.Renderer.transform.parent.GetComponent<VoteSpreader>().Votes.Remove(vote.Renderer);
            Object.Destroy(vote.Renderer);
        }
    }

    private static void UpdateAllCachedVoteSpreaders(this MeetingHud meetingHud)
    {
        var shouldRevealVotes = LocalConditions.ShouldRevealVotes();
        var shouldRevealVoteColors = LocalConditions.ShouldRevealVoteColors();
        
        var shouldUpdateColor = PreviousDisplayColorState != shouldRevealVoteColors;
        if (shouldUpdateColor)
        {
            PreviousDisplayColorState = shouldRevealVoteColors;
        }
        if (meetingHud.SkippedVoting)
        {
            meetingHud.SkippedVoting.SetActive(shouldRevealVotes);
        }
        foreach (var vote in CachedVotes)
        {
            var created = false;
            if (!vote.Renderer)
            {
                meetingHud.CreateBetterVoteIcon(vote);
                created = true;
            }
            if ((shouldUpdateColor || created) && vote.Renderer)
            {
                if (!shouldRevealVoteColors)
                {
                    PlayerMaterial.SetColors(Palette.DisabledGrey, vote.Renderer);
                }
                else if (vote.Voter.Player != null)
                {
                    PlayerMaterial.SetColors(vote.Voter.Player.Data.DefaultOutfit.ColorId, vote.Renderer);
                }
            }
            if (vote.Renderer != null)
            {
                vote.Renderer.enabled = shouldRevealVotes;
            }
        }
    }

    private static void CreateBetterVoteIcon(this MeetingHud meetingHud, VoteData vote)
    {
        var votedPva = vote.Voted != null ? meetingHud.playerStates.FirstOrDefault(x => x && x.TargetPlayerId == vote.Voted.Player?.PlayerId) : null;
        var parent = votedPva != null ? votedPva.transform : meetingHud.SkippedVoting.transform;
        if (!parent)
        {
            Ls.LogWarning($"No transform found");
            return;
        }
        var voteIcon = Object.Instantiate(meetingHud.PlayerVotePrefab, parent);
        PlayerMaterial.SetColors(Palette.DisabledGrey, voteIcon);
        voteIcon.transform.localPosition = Vector3.zero;
        var pva = parent.GetComponent<PlayerVoteArea>();
        if (pva)
        {
            voteIcon.material.SetInt(PlayerMaterial.MaskLayer, pva.MaskLayer);
        }
        parent.GetComponent<VoteSpreader>().AddVote(voteIcon);
        vote.Renderer = voteIcon;
    }

    private class VoteData(BetterPlayerControl voter, BetterPlayerControl? voted)
    {
        public readonly BetterPlayerControl Voter = voter;
        public readonly BetterPlayerControl? Voted = voted;
        public SpriteRenderer? Renderer;
    }
    
    private static void LogVote(BetterPlayerControl voter, BetterPlayerControl? suspect)
    {
        if (!LocalConditions.ShouldRevealVotes() || !LocalConditions.ShouldRevealVoteColors()) return;
        Ls.LogMessage($"{voter.Player?.Data.PlayerName} voted for {(suspect != null ? suspect.Player?.Data.PlayerName : "skip")}");
    }

    public static void HideDeadPlayerPets(this MeetingHud meetingHud)
    {
        meetingHud.StartCoroutine(CoHideDeadPlayerPets());
    }

    private static IEnumerator CoHideDeadPlayerPets()
    {
        if (HostOptions.Default.HideDeadPlayerPets.Value && LocalConditions.AmHost())
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || player.Data == null || !player.Data.IsDead) continue;
                player.RpcHidePet();
                yield return new WaitForSeconds(1f);
            }
        }
        else if (LocalOptions.Default.HideMyPetAfterDeath.Value)
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                yield break;
            }
            PlayerControl.LocalPlayer.RpcHidePet();
        }
    }
}