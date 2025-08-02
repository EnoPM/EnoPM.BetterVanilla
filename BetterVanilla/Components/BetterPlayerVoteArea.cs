using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterPlayerVoteArea : MonoBehaviour
{
    public static readonly List<BetterPlayerVoteArea> AllVoteAreas = [];
    public PlayerVoteArea VoteArea { get; private set; } = null!;
    private BetterPlayerTexts? InfosText { get; set; }

    private void Awake()
    {
        VoteArea = GetComponent<PlayerVoteArea>();
        AllVoteAreas.Add(this);
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private void OnDestroy()
    {
        AllVoteAreas.Remove(this);
    }

    private IEnumerator CoStart()
    {
        InfosText = Instantiate(BetterVanillaManager.Instance.BetterVoteAreaTextsPrefab, VoteArea.NameText.transform.parent);
        InfosText.gameObject.hideFlags |= HideFlags.HideAndDontSave;
        InfosText.gameObject.SetActive(true);

        while (!InfosText.IsReady)
        {
            yield return new WaitForEndOfFrame();
        }
        
        
        var pos = VoteArea.NameText.transform.localPosition;
        //pos.y = 0.2f;
        InfosText.transform.localPosition = pos;
        InfosText.SetSponsorText("Sponsor");
        InfosText.SetMainText("Main Text");
    }

    private void Update()
    {
        if (InfosText == null || !MeetingHud.Instance || !InfosText.IsReady) return;
        if (MeetingHud.Instance.state is MeetingHud.VoteStates.Animating or MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
        {
            InfosText.gameObject.SetActive(false);
            return;
            
        }
        var player = BetterVanillaManager.Instance.AllPlayers.Find(x => x.Player.PlayerId == VoteArea.TargetPlayerId);
        if (player == null)
        {
            InfosText.gameObject.SetActive(false);
            return;
        }
        InfosText.SetSponsorText(player.GetSponsorText());
        InfosText.SetMainText(player.GetBetterInfosText());
        InfosText.gameObject.SetActive(true);
    }
}