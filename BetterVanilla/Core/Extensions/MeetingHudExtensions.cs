using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MeetingHudExtensions
{
    private static readonly List<VoteData> CachedVotes = [];
    private static bool PreviousDisplayColorState { get; set; }

    public static void BetterStart(this MeetingHud meetingHud)
    {
        CachedVotes.Clear();
    }

    public static void BetterCastVote(this MeetingHud meetingHud, byte voterId, byte votedId)
    {
        if (!AmongUsClient.Instance.AmHost || !HostOptions.Default.AllowDeadVoteDisplay.Value) return;
        var voter = BetterVanillaManager.Instance.GetPlayerById(voterId);
        if (!voter)
        {
            Ls.LogWarning($"Player {voterId} has not been found in BetterCastVote.");
            return;
        }
        PlayerControl.LocalPlayer.RpcSetMeetingVote(voterId, votedId);
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
        foreach (var vote in CachedVotes.Where(x => x.Renderer))
        {
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
                else
                {
                    PlayerMaterial.SetColors(vote.Voter.Player.Data.DefaultOutfit.ColorId, vote.Renderer);
                }
            }
            if (vote.Renderer)
            {
                vote.Renderer.enabled = shouldRevealVotes;
            }
        }
    }

    private static void CreateBetterVoteIcon(this MeetingHud meetingHud, VoteData vote)
    {
        var votedPva = vote.Voted ? meetingHud.playerStates.FirstOrDefault(x => x && x.TargetPlayerId == vote.Voted.Player.PlayerId) : null;
        var parent = votedPva ? votedPva.transform : meetingHud.SkippedVoting.transform;
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

    private class VoteData(BetterPlayerControl voter, BetterPlayerControl voted)
    {
        public readonly BetterPlayerControl Voter = voter;
        public readonly BetterPlayerControl Voted = voted;
        public SpriteRenderer Renderer;
    }
    
    private static void LogVote(BetterPlayerControl voter, BetterPlayerControl suspect)
    {
        if (!LocalConditions.ShouldRevealVotes() || !LocalConditions.ShouldRevealVoteColors()) return;
        Ls.LogMessage($"{voter.Player?.Data.PlayerName} voted for {(suspect != null ? suspect.Player?.Data.PlayerName : "skip")}");
    }
}