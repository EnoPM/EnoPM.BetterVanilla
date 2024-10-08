﻿using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Core.Helpers;
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
        var voter = BetterVanillaManager.Instance.GetPlayerById(voterId);
        var voted = BetterVanillaManager.Instance.GetPlayerById(votedId);

        if (!voter)
        {
            Ls.LogError($"Unable to find voter by id {voterId}");
            return;
        }
        
        LogVote(voter, voted);

        if (CachedVotes.Any(x => x.Voter == voter))
        {
            Ls.LogError($"{voter.Player.Data.PlayerName} vote is already cached");
            return;
        }

        CachedVotes.Add(new VoteData(voter, voted));
    }

    public static void BetterUpdate(this MeetingHud meetingHud)
    {
        var localOptions = BetterVanillaManager.Instance.LocalOptions;
        var shouldRevealVotes = localOptions.DisplayVotesAfterDeath.Value && ConditionUtils.AmDead();
        var shouldRevealVoteColors = localOptions.DisplayVoteColorsAfterDeath.Value && ConditionUtils.AmDead();
        
        if (meetingHud.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Proceeding)
        {
            foreach (var vote in CachedVotes.Where(x => x.Renderer))
            {
                vote.Renderer.transform.parent.GetComponent<VoteSpreader>().Votes.Remove(vote.Renderer);
                Object.Destroy(vote.Renderer);
            }
        }
        else
        {
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
        var localOptions = BetterVanillaManager.Instance.LocalOptions;
        if (ConditionUtils.AmDead() && localOptions.DisplayVoteColorsAfterDeath.Value && localOptions.DisplayVotesAfterDeath.Value)
        {
            Ls.LogMessage($"{voter.Player.Data.PlayerName} voted for {(suspect ? suspect.Player.Data.PlayerName : "skip")}");
        }
    }
}