using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Options;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Components;

public class BetterPlayerControl : MonoBehaviour
{
    public static BetterPlayerControl? LocalPlayer { get; private set; }
    
    public PlayerControl? Player { get; private set; }
    public BetterPlayerTexts? PlayerTexts { get; private set; }
    public string? FriendCode { get; private set; }
    public bool AmSponsor { get; private set; }

    public Color? VisorColor { get; private set; }
    public string? SponsorText { get; private set; }
    public Color? SponsorColor { get; private set; }

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
            Ls.LogMessage($"[BetterPlayerControl] LocalPlayer: {Player.Data.PlayerName}");
            LocalPlayer = this;
            FriendCode = EOSManager.Instance.FriendCode;
            UpdateSponsorState();
            Ls.LogMessage($"[BetterPlayerControl] LocalPlayer sponsor state: {AmSponsor}");
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
        return Instantiate(BetterVanillaManager.Instance.PlayerTextsPrefab, Player.cosmetics.nameText.transform);
    }

    private TextMeshPro CreateVanillaInfosText()
    {
        var infosText = Instantiate(Player.cosmetics.nameText, Player.cosmetics.nameText.transform.parent);
        var pos = infosText.transform.localPosition;
        pos.y = 0.2f;
        infosText.transform.localPosition = pos;
        infosText.SetText(string.Empty);
        return infosText;
    }

    private void OnBodyColorUpdated(int bodyColor)
    {
        Ls.LogMessage($"{Player.Data.PlayerName} - Body color: {bodyColor}");
        RefreshVisorColor();
    }

    private void Update()
    {
        if (Player.Data && Player.cosmetics && PlayerTexts && PlayerTexts.IsReady)
        {
            var isActive = !LocalConditions.IsGameStarted() || LocalConditions.AmDead() || Player.AmOwner;
            PlayerTexts.gameObject.SetActive(isActive);
            if (isActive)
            {
                PlayerTexts.SetMainText(GetBetterInfosText());
                PlayerTexts.SetSponsorText(GetSponsorText());
            }
        }
        if (AmSponsor && Player && Player.AmOwner)
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
        if (!AmSponsor || SponsorColor == null || SponsorText == null) return string.Empty;
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
        if (LobbyBehaviour.Instance) return;
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
        if (!role) return;
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
        Ls.LogMessage($"Friend code for player {Player.Data.PlayerName}: {FriendCode}");
        UpdateSponsorState();
    }
}