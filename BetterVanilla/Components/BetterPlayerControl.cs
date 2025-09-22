using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Core.Rpc;
using BetterVanilla.Cosmetics;
using BetterVanilla.Options;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Components;

public class BetterPlayerControl : MonoBehaviour
{
    public static BetterPlayerControl? LocalPlayer { get; private set; }

    public PlayerControl Player { get; private set; } = null!;
    public string? FriendCode { get; private set; }
    public bool AmSponsor { get; private set; }
    private BetterPlayerTexts? PlayerTexts { get; set; }
    private Color? VisorColor { get; set; }
    private string? SponsorText { get; set; }
    private Color? SponsorColor { get; set; }
    public BetterVanillaHandshake? Handshake { get; private set; }
    private string? RoleName { get; set; }

    private void Awake()
    {
        Player = GetComponent<PlayerControl>();
        BetterVanillaManager.Instance.AllPlayers.Add(this);
        if (GetComponent<DummyBehaviour>() != null)
        {
            SetHandshake(BetterVanillaHandshake.Local);
        }
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
        if (!Player.AmOwner) return;
        LocalPlayer = this;
        Handshake = BetterVanillaHandshake.Local;
        FriendCode = EOSManager.Instance.FriendCode;
        UpdateSponsorState();
    }

    private void OnDestroy()
    {
        BetterVanillaManager.Instance.AllPlayers.Remove(this);
        if (LocalPlayer == this)
        {
            LocalPlayer = null;
        }
    }

    private IEnumerator CoStart()
    {
        while (Player.Data == null || !Player.cosmetics || !Player.cosmetics.nameText)
        {
            yield return null;
        }

        PlayerTexts = CreateBetterInfosTexts();
        PlayerTexts.gameObject.SetActive(true);
        while (string.IsNullOrWhiteSpace(Player.Data.PlayerName))
        {
            yield return new WaitForEndOfFrame();
        }

        while (!Player.cosmetics.initialized)
        {
            yield return new WaitForEndOfFrame();
        }
        Player.cosmetics.add_OnColorChange(new Action<int>(OnBodyColorUpdated));
        RefreshVisorColor();
        
        Ls.LogMessage($"{nameof(BetterPlayerControl)} - Player ready: {Player.Data.PlayerName} - {FriendCode}");
    }

    private BetterPlayerTexts CreateBetterInfosTexts()
    {
        return Instantiate(BetterVanillaManager.Instance.PlayerTextsPrefab, Player.cosmetics.nameText.transform);
    }

    private void OnBodyColorUpdated(int bodyColor)
    {
        RefreshVisorColor();
    }

    private void Update()
    {
        if (PlayerTexts != null && Player.Data && Player.cosmetics && PlayerTexts && PlayerTexts.IsReady)
        {
            var isActive = !LocalConditions.IsGameStarted() || LocalConditions.AmDead() || Player.AmOwner;
            PlayerTexts.gameObject.SetActive(isActive);
            if (isActive)
            {
                var infos = GetBetterInfosText();
                if (string.IsNullOrEmpty(infos))
                {
                    PlayerTexts.SetMainTextActive(false);
                }
                else
                {
                    PlayerTexts.SetMainTextActive(true);
                    PlayerTexts.SetMainText(infos);
                }
                var sponsorText = GetSponsorText();
                if (string.IsNullOrEmpty(sponsorText) || !AmSponsor)
                {
                    PlayerTexts.SetSponsorTextActive(false);
                }
                else
                {
                    PlayerTexts.SetSponsorTextActive(true);
                    PlayerTexts.SetSponsorText(sponsorText);
                }
                if (Handshake == null || !LocalOptions.Default.DisplayBetterVanillaVersion.Value)
                {
                    PlayerTexts.SetHandshakeTextActive(false);
                }
                else
                {
                    PlayerTexts.SetHandshakeTextActive(true);
                    PlayerTexts.SetHandshakeText(Handshake);
                }
            }
        }
        if (AmSponsor && Player.AmOwner)
        {
            if (SponsorOptions.Default.VisorColor.Value != VisorColor)
            {
                VisorColor = SponsorOptions.Default.VisorColor.Value;
            }
            if (SponsorOptions.Default.SponsorText.Value != SponsorText)
            {
                SponsorText = SponsorOptions.Default.SponsorText.Value;
            }
            if (SponsorOptions.Default.SponsorTextColor.Value != SponsorColor)
            {
                SponsorColor = SponsorOptions.Default.SponsorTextColor.Value;
            }
        }

        if (AmSponsor)
        {
            RefreshVisorColor();
        }
    }

    private bool IsCheater()
    {
        return BetterVanillaManager.Instance.Cheaters.IsCheating(Player);
    }

    private (int, int) GetTasksCount()
    {
        var total = 0;
        var done = 0;
        if (Player.Data.Tasks != null)
        {
            foreach (var task in Player.Data.Tasks)
            {
                total++;
                if (task.Complete)
                {
                    done++;
                }
            }
        }

        return (done, total);
    }

    private void SetupCheaterInfoText(ref List<string> infos)
    {
        if (!IsCheater()) return;
        infos.Add(ColorUtils.ColoredString(ColorUtils.CheaterColor, "Cheater"));
    }

    public string GetSponsorText()
    {
        if (!AmSponsor || SponsorColor == null || string.IsNullOrWhiteSpace(SponsorText)) return string.Empty;
        return ColorUtils.ColoredString(SponsorColor.Value, SponsorText, false);
    }

    public void UpdateSponsorState()
    {
        if (string.IsNullOrWhiteSpace(FriendCode) || FeatureCodeBehaviour.Instance == null || FeatureCodeBehaviour.Instance.Registry == null)
        {
            return;
        }
        var value = FeatureCodeBehaviour.Instance.Registry.ContributorFriendCodes.Contains(FriendCode);
        if (value == AmSponsor) return;
        Ls.LogMessage($"Updating sponsor state for player {FriendCode} ({Player.Data?.PlayerName}) : {value}");
        AmSponsor = value;
    }

    public void SetSponsorText(string sponsorText)
    {
        SponsorText = sponsorText;
    }

    public void SetSponsorTextColor(Color sponsorTextColor)
    {
        SponsorColor = sponsorTextColor;
    }

    public void SetVisorColor(Color? color)
    {
        VisorColor = color;
        RefreshVisorColor();
    }

    private void RefreshVisorColor()
    {
        if (!AmSponsor || Player.cosmetics == null || !Player.cosmetics.initialized) return;
        Player.cosmetics.currentBodySprite.BodySprite.SetVisorColor(GetVisorColor());
        RefreshHatColor();
        //RefreshHostPanel();
        //RefreshCustomizationMenu();
    }
    
    public Color GetVisorColor() => AmSponsor ? VisorColor ?? Palette.VisorColor : Palette.VisorColor;

    public void RefreshHatColor()
    {
        if (Player.cosmetics == null || !Player.cosmetics.initialized || Player.cosmetics.hat == null || Player.cosmetics.hat.Hat == null) return;
        if (CosmeticsManager.Hats.TryGetViewData(Player.cosmetics.hat.Hat.ProductId, out var viewData) && viewData.MatchPlayerColor)
        {
            Player.cosmetics.hat.FrontLayer.SetVisorColor(GetVisorColor());
        }
    }

    private void SetupHostOrDisconnectedInfoText(ref List<string> infos)
    {
        if (LobbyBehaviour.Instance != null) return;
        if (AmongUsClient.Instance && Player.OwnerId == AmongUsClient.Instance.HostId)
        {
            infos.Add(ColorUtils.ColoredString(ColorUtils.HostColor, "Host"));
        }
        else if (Player.Data.Disconnected)
        {
            infos.Add(ColorUtils.ColoredString(ColorUtils.ImpostorColor, "Disconnected"));
        }
    }

    private void SetupRoleOrTaskInfoText(ref List<string> infos)
    {
        if (!LocalConditions.ShouldShowRolesAndTasks(Player)) return;
        var role = Player.Data.Role;
        if (role == null) return;
        if (role.IsImpostor)
        {
            var roleName = Player.Data.IsDead ? StringNames.Impostor : role.StringName;
            infos.Add(ColorUtils.ColoredString(ColorUtils.ImpostorColor, TranslationController.Instance.GetString(roleName)));
        }
        else
        {
            var (done, total) = GetTasksCount();
            var color = ColorUtils.TaskCountColor(done, total);
            var crewmateRoleName = GetCrewmateRoleName();
            if (!string.IsNullOrEmpty(crewmateRoleName))
            {
                infos.Add(ColorUtils.ColoredString(Palette.CrewmateBlue, crewmateRoleName));
            }
            infos.Add(ColorUtils.ColoredString(color, $"{done}/{total}"));
        }
    }

    private string? GetCrewmateRoleName()
    {
        if (!LocalConditions.IsGameStarted()) return null;
        if (!string.IsNullOrEmpty(RoleName)) return RoleName;
        if (Player.Data == null || Player.Data.Role == null || Player.Data.Role.IsImpostor) return null;
        var role = Player.Data.Role;
        var roleName = GetCrewmateRoleName(role.Role);
        var crewmateRoleName = GetCrewmateRoleName(RoleTypes.Crewmate);
        if (crewmateRoleName == roleName) return roleName;
        return RoleName = roleName;
    }

    private static string? GetCrewmateRoleName(RoleTypes role)
    {
        switch (role)
        {
            case RoleTypes.Detective:
                return "Det.";
            case RoleTypes.Scientist:
                return "Sci.";
            case RoleTypes.Engineer:
                return "Eng.";
            case RoleTypes.GuardianAngel:
                return "G.A.";
            case RoleTypes.Noisemaker:
                return "Nois.";
            case RoleTypes.Tracker:
                return "Trac.";
            case RoleTypes.CrewmateGhost:
            case RoleTypes.Crewmate:
                return "Crew.";
        }
        return null;
    }

    public string GetBetterInfosText()
    {
        var infos = new List<string>();
        SetupCheaterInfoText(ref infos);
        SetupHostOrDisconnectedInfoText(ref infos);
        SetupRoleOrTaskInfoText(ref infos);
        return string.Join(" - ", infos);
    }

    public void SetFriendCode(string friendCode)
    {
        if (FriendCode == friendCode || string.IsNullOrEmpty(friendCode)) return;
        FriendCode = friendCode;
        Ls.LogMessage($"Setting friend code for player {Player.Data.PlayerName}: {FriendCode}");
        UpdateSponsorState();
    }

    public void SetHandshake(BetterVanillaHandshake handshake)
    {
        Handshake = handshake;
    }

    public IEnumerator CoOwnerSpawnHandshake()
    {
        yield return new WaitForSeconds(0.5f);
        
        RpcSetHandshake(BetterVanillaHandshake.Local);
        yield return new WaitForSeconds(0.1f);
        
        RpcSetTeamPreference(LocalOptions.Default.TeamPreference.ParseValue(TeamPreferences.Both));
        yield return new WaitForSeconds(0.1f);
        
        if (FeatureOptions.Default.ForcedTeamAssignment.IsAllowed())
        {
            RpcSetForcedTeamAssignment(FeatureOptions.Default.ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
            yield return new WaitForSeconds(0.1f);
        }

        if (LocalConditions.AmHost())
        {
            HostOptions.Default.ShareAllOptions();
            yield return new WaitForSeconds(0.1f);
        }

        if (!AmSponsor) yield break;
        
        RpcSetSponsorText(SponsorOptions.Default.SponsorText.Value);
        yield return new WaitForSeconds(0.1f);
            
        RpcSetSponsorTextColor(SponsorOptions.Default.SponsorTextColor.Value);
        yield return new WaitForSeconds(0.1f);
            
        RpcSetVisorColor(SponsorOptions.Default.VisorColor.Value);
        yield return new WaitForSeconds(0.1f);
    }

    public static IEnumerator CoSpawnHandshake()
    {
        yield return new WaitForSeconds(0.5f);
        if (LocalPlayer == null) yield break;
        
        LocalPlayer.RpcSetHandshake(BetterVanillaHandshake.Local);
        yield return new WaitForSeconds(0.1f);
        
        if (LocalConditions.AmHost())
        {
            HostOptions.Default.ShareAllOptions();
            yield return new WaitForSeconds(0.1f);
        }
        LocalPlayer.RpcSetTeamPreference(LocalOptions.Default.TeamPreference.ParseValue(TeamPreferences.Both));
        yield return new WaitForSeconds(0.1f);
        
        if (LocalPlayer.Player != null && !LocalPlayer.Player.AmOwner && FeatureOptions.Default.ForcedTeamAssignment.IsAllowed())
        {
            LocalPlayer.RpcSetForcedTeamAssignment(FeatureOptions.Default.ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
            yield return new WaitForSeconds(0.1f);
        }
        if (!LocalConditions.AmSponsor()) yield break;
        
        SponsorOptions.Default.ShareSponsorText();
        yield return new WaitForSeconds(0.1f);
        
        SponsorOptions.Default.ShareSponsorTextColor();
        yield return new WaitForSeconds(0.1f);
        
        SponsorOptions.Default.ShareVisorColor();
        yield return new WaitForSeconds(0.1f);
    }

    public void RpcSetHandshake(BetterVanillaHandshake handshake)
    {
        var rpc = new HandshakeRpc(this, handshake);
        rpc.Send();
    }

    public void RpcSetSponsorText(string sponsorText)
    {
        var rpc = new SponsorTextRpc(this, sponsorText);
        rpc.Send();
    }

    public void RpcSetSponsorTextColor(Color sponsorTextColor)
    {
        var rpc = new SponsorTextColorRpc(this, sponsorTextColor);
        rpc.Send();
    }

    public void RpcSetVisorColor(Color color)
    {
        var rpc = new SponsorVisorColorRpc(this, color);
        rpc.Send();
    }

    public void RpcSetMeetingVote(byte voterId, byte votedId)
    {
        var rpc = new MeetingVoteRpc(this, voterId, votedId);
        rpc.Send();
    }

    public void RpcSetTeamPreference(TeamPreferences preference)
    {
        var rpc = new TeamPreferenceRpc(this, preference);
        rpc.Send();
    }

    public void RpcSetForcedTeamAssignment(TeamPreferences preference)
    {
        var rpc = new ForcedTeamAssignmentRpc(this, preference);
        rpc.Send();
    }

    public void RpcSetHostOptionValue(AbstractSerializableOption option)
    {
        var rpc = new HostOptionRpc(this, option);
        rpc.Send();
    }

    public void RpcShareAllHostOptions()
    {
        var rpc = new ShareAllHostOptionsRpc(this, HostOptions.Default.ToBytes());
        rpc.Send();
    }

    public void RpcSendPrivateChatMessage(int receiverOwnerId, string message)
    {
        var rpc = new PrivateChatMessageRpc(this, receiverOwnerId, message);
        rpc.Send();
    }

    public void RpcSetFirstKilledPlayer(string friendCode)
    {
        var rpc = new ShareFirstKilledPlayerRpc(this, friendCode);
        rpc.Send();
    }

    public void RpcShareRandomizedMeetingPositions()
    {
        if (!LocalConditions.AmHost()
            || MeetingHud.Instance == null
            || HostOptions.Default.RandomizePlayerOrderInMeetings.IsNotAllowed()
            || !HostOptions.Default.RandomizePlayerOrderInMeetings.Value)
        {
            return;
        }
        var positions = MeetingHud.Instance.playerStates
            .Where(x => !x.AmDead && !x.DidReport)
            .Select(x => x.TargetPlayerId)
            .ToList();
        positions.Shuffle();
        var rpc = new RandomizedMeetingOrderRpc(this, positions.ToArray());
        rpc.Send();
    }
}