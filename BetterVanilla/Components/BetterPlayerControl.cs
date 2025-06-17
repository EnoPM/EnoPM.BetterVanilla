using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Helpers;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Components;

public class BetterPlayerControl : MonoBehaviour
{
    public PlayerControl Player { get; private set; }
    public TextMeshPro InfosText { get; private set; }
    public string FriendCode { get; private set; }
    public bool AmSponsor { get; private set; }
    
    public Color? VisorColor { get; private set; }

    private void Awake()
    {
        Player = GetComponent<PlayerControl>();
        BetterVanillaManager.Instance.AllPlayers.Add(this);
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private void OnDestroy()
    {
        BetterVanillaManager.Instance.AllPlayers.Remove(this);
    }

    private IEnumerator CoStart()
    {
        while (!Player || !Player.Data || !Player.cosmetics || !Player.cosmetics.nameText)
        {
            yield return null;
        }
        
        InfosText = Instantiate(Player.cosmetics.nameText, Player.cosmetics.nameText.transform.parent);
        var pos = InfosText.transform.localPosition;
        pos.y = 0.2f;
        InfosText.transform.localPosition = pos;
        InfosText.SetText(string.Empty);
        while (string.IsNullOrWhiteSpace(Player.Data.PlayerName))
        {
            yield return new WaitForEndOfFrame();
        }
        Ls.LogMessage($"{nameof(BetterPlayerControl)} - Player ready: {Player.Data.PlayerName}");
        Player.cosmetics.add_OnColorChange(new Action<int>(OnBodyColorUpdated));
    }

    private void OnBodyColorUpdated(int bodyColor)
    {
        Ls.LogMessage($"{Player.Data.PlayerName} - Body color: {bodyColor}");
        RefreshVisorColor();
    }

    private void Update()
    {
        if (Player.Data && Player.cosmetics && InfosText)
        {
            var isActive = LocalConditions.AmDead() || Player.AmOwner || !LocalConditions.IsGameStarted();
            InfosText.gameObject.SetActive(isActive);
            if (isActive)
            {
                InfosText.SetText(GetBetterInfosText());
            }
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

    private void SetupSponsorInfoText(ref List<string> infos)
    {
        if (!AmSponsor) return;
        infos.Add(ColorUtils.ColoredString(Color.blue, "Sponsor"));
    }

    public void UpdateSponsorState()
    {
        if (string.IsNullOrWhiteSpace(FriendCode) || BetterVanillaManager.Instance.Features.Registry == null)
        {
            return;
        }
        AmSponsor = BetterVanillaManager.Instance.Features.Registry.ContributorFriendCodes.Contains(FriendCode);
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
        SetupSponsorInfoText(ref infos);
        SetupRoleOrTaskInfoText(ref infos);
        return $"<size=70%>{string.Join(" - ", infos)}</size>";
    }

    public void SetFriendCode(string friendCode)
    {
        FriendCode = friendCode;
        Ls.LogMessage($"Friend code for player {Player.Data.PlayerName}: {FriendCode}");
    }
}