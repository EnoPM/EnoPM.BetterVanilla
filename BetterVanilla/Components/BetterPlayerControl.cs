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
    }

    private void Update()
    {
        if (Player.Data && Player.cosmetics && InfosText)
        {
            var isActive = ConditionUtils.AmDead() || Player.AmOwner || !ConditionUtils.IsGameStarted();
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

    public string GetBetterInfosText()
    {
        var role = Player.Data.Role;
        var amDead = ConditionUtils.AmDead();
        var shouldShowRolesAndTasks = Player == PlayerControl.LocalPlayer || BetterVanillaManager.Instance.LocalOptions.DisplayRolesAndTasksAfterDeath.Value;
        var isHost = Player.OwnerId == AmongUsClient.Instance.HostId;
        var isDisconnected = Player.Data.Disconnected;
        var isCheater = IsCheater();
        var texts = new List<string>();
        if (isCheater)
        {
            texts.Add(ColorUtils.ColoredString(ColorUtils.CheaterColor, "Cheater"));
        }
        if (isHost)
        {
            texts.Add(ColorUtils.ColoredString(ColorUtils.HostColor, "Host"));
        }
        else if (isDisconnected)
        {
            texts.Add(ColorUtils.ColoredString(ColorUtils.ImpostorColor, "Disconnected"));
        }
        if (shouldShowRolesAndTasks && role && ConditionUtils.IsGameStarted() && (amDead || Player == PlayerControl.LocalPlayer))
        {
            if (role.IsImpostor)
            {
                texts.Add(ColorUtils.ColoredString(ColorUtils.ImpostorColor, TranslationController.Instance.GetString(StringNames.Impostor)));
            }
            else
            {
                var (done, total) = GetTasksCount();
                var half = Mathf.RoundToInt(total / 2f);
                var color = ColorUtils.NoTasksDoneColor;
                if (done > 0 && done < half)
                {
                    color = ColorUtils.LessThanHalfTasksDoneColor;
                }
                else if (done >= total)
                {
                    color = ColorUtils.AllTasksDoneColor;
                }
                else if (done > 0 && done >= half)
                {
                    color = ColorUtils.MoreThanHalfTasksDoneColor;
                }
                texts.Add(ColorUtils.ColoredString(color, $"{done}/{total}"));
            }
        }


        return $"<size=70%>{string.Join(" - ", texts)}</size>";
    }
}