using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Components;

public class BetterPlayerControl : MonoBehaviour
{
    public static BetterPlayerControl? LocalPlayer { get; private set; }
    
    public PlayerControl? Player { get; private set; }
    public string? FriendCode { get; private set; }
    public bool AmSponsor { get; private set; }
    private BetterPlayerTexts? PlayerTexts { get; set; }
    private Color? VisorColor { get; set; }
    private string? SponsorText { get; set; }
    private Color? SponsorColor { get; set; }
    public BetterVanillaHandshake? Handshake { get; private set; }
    public bool IsProtected { get; set; }

    private void Awake()
    {
        Player = GetComponent<PlayerControl>();
        BetterVanillaManager.Instance.AllPlayers.Add(this);
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
        if (Player != null && Player.AmOwner)
        {
            LocalPlayer = this;
            Handshake = BetterVanillaHandshake.Local;
            FriendCode = EOSManager.Instance.FriendCode;
            UpdateSponsorState();
        }
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
        while (Player == null || Player.Data == null || !Player.cosmetics || !Player.cosmetics.nameText)
        {
            yield return null;
        }

        PlayerTexts = CreateBetterInfosTexts();
        PlayerTexts.gameObject.SetActive(true);
        while (string.IsNullOrWhiteSpace(Player.Data.PlayerName))
        {
            yield return new WaitForEndOfFrame();
        }
        Ls.LogMessage($"{nameof(BetterPlayerControl)} - Player ready: {Player.Data.PlayerName}");
        Player.cosmetics.add_OnColorChange(new Action<int>(OnBodyColorUpdated));
    }

    private BetterPlayerTexts CreateBetterInfosTexts()
    {
        return Instantiate(BetterVanillaManager.Instance.PlayerTextsPrefab, Player!.cosmetics.nameText.transform);
    }

    private void OnBodyColorUpdated(int bodyColor)
    {
        Ls.LogMessage($"{Player?.Data.PlayerName} - Body color: {bodyColor}");
        RefreshVisorColor();
    }

    private void Update()
    {
        if (Player != null && PlayerTexts != null && Player.Data && Player.cosmetics && PlayerTexts && PlayerTexts.IsReady)
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
            if (IsProtected)
            {
                Player.cosmetics.nameText.SetText($"{Player.Data.PlayerName}\n<size=50%>Protected ({Mathf.RoundToInt(PlayerShieldBehaviour.Instance.Timer)}s)</size>");
            }
            else
            {
                Player.cosmetics.nameText.SetText(Player.Data.PlayerName);
            }
        }
        if (AmSponsor && Player != null && Player.AmOwner)
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
        if (Player?.Data.Tasks != null)
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
        Ls.LogMessage($"{nameof(BetterPlayerControl)} - Updating sponsor state");
        if (string.IsNullOrWhiteSpace(FriendCode) || FeatureCodeBehaviour.Instance == null)
        {
            return;
        }
        AmSponsor = FeatureCodeBehaviour.Instance.SponsorFriendCodes.Contains(FriendCode);
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
        if (Player == null || Player.cosmetics == null) return;
        var color = VisorColor ?? Palette.VisorColor;
        Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(PlayerMaterial.VisorColor, color);
    }

    private void SetupHostOrDisconnectedInfoText(ref List<string> infos)
    {
        if (LobbyBehaviour.Instance != null || Player == null) return;
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
        if (Player == null) return;
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
            infos.Add(ColorUtils.ColoredString(color, $"{done}/{total}"));
        }
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
        FriendCode = friendCode;
        Ls.LogMessage($"Friend code for player {Player?.Data.PlayerName}: {FriendCode}");
        UpdateSponsorState();
    }

    public void SetHandshake(BetterVanillaHandshake handshake)
    {
        Handshake = handshake;
    }
}